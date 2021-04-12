using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Models.User;
using MyUniversity.UserManager.Repository.DbContext;
using MyUniversity.UserManager.Repository.Entities.User;
using MyUniversity.UserManager.Repository.Helpers;
using MyUniversity.UserManager.Services.Settings;

namespace MyUniversity.UserManager.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UMDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenDecoder _tokenDecoder;
        private readonly IPermissionResolver _permissionResolver;

        public UserService(
            ILogger<UserService> logger,
            UMDBContext dBContext,
            IMapper mapper,
            IOptions<JwtSettings> jwtSettings,
            ITokenDecoder tokenDecoder,
            IPermissionResolver permissionResolver)
        {
            _logger = logger;
            _dBContext = dBContext;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
            _tokenDecoder = tokenDecoder;
            _permissionResolver = permissionResolver;
        }

        public async Task<UserModel> RegisterUserAsync(RegisterUserModel userModel, string accessToken)
        {
            _logger.LogDebug($"Check if the user with email {userModel.EmailAddress} already exists in the system");

            var userEntity = await _dBContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == userModel.EmailAddress);

            if (userEntity is not null)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, $"User with email {userModel.EmailAddress} already exists"));
            }

            _logger.LogDebug("Validation of new user roles");

            var newUserRoles = await _dBContext.Roles.Where(e => userModel.Roles.Any(roleId => roleId == e.Id)).ToListAsync();

            if (newUserRoles.Count != userModel.Roles.Count())
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not all assigned roles were found"));
            }

            if (!_permissionResolver.CanUserCreateUserWithRoles(newUserRoles, accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "You do not have permissions to create this user"));
            }

            _logger.LogDebug("Validation of new user tenant");

            var newUserTenant = string.IsNullOrWhiteSpace(userModel.UniversityId)
                ? null
                : await _dBContext.Universities.FirstOrDefaultAsync(x => x.TenantId == userModel.UniversityId);

            if (!_permissionResolver.CanUserExistWithoutTenant(newUserRoles) && newUserTenant is null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "This user cannot exist without university"));
            }

            _logger.LogDebug("Creating new user");

            PasswordHashHelper.CreatePasswordHash(userModel.Password, out var hash, out var salt);

            userEntity = new UserEntity
            {
                FirstName = userModel.LastName,
                LastName = userModel.LastName,
                EmailAddress = userModel.EmailAddress,
                PhoneNumber = userModel.PhoneNumber,
                TenantId = newUserTenant?.TenantId,
                UserRoles = newUserRoles.Select(x => new UserRoleEntity { RoleId = x.Id }).ToList(),
                IsSoftDeleted = false,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            await _dBContext.Users.AddAsync(userEntity);
            await _dBContext.SaveChangesAsync();

            return _mapper.Map<UserModel>(userEntity);
        }

        public async Task<string> LoginUserAsync(string email, string password)
        {
            _logger.LogDebug($"Search user with email {email}");

            var user = await _dBContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.EmailAddress == email);

            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Invalid email or password"));
            }

            _logger.LogDebug("Validation of user access");

            var isUserVerified = PasswordHashHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);

            if (!isUserVerified)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Invalid email or password"));
            }

            _logger.LogDebug("Creating user token");

            return CreateToken(user);
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync(string accessToken)
        {
            _logger.LogDebug("Preparing db query");

            var query = CreateGetUsersQuery(accessToken);

            _logger.LogDebug("Executing db query");

            var users = await query.ToListAsync();

            _logger.LogDebug($"Were found {users.Count} users");

            return users.Select(_mapper.Map<UserModel>);
        }

        public async Task<UserModel> GetUserByIdAsync(int id, string accessToken)
        {
            _logger.LogDebug("Preparing db query");

            var query = CreateGetUsersQuery(accessToken);

            _logger.LogDebug("Executing db query");

            var user = await query.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User with id {id} not found"));
            }

            return _mapper.Map<UserModel>(user);
        }

        private IQueryable<UserEntity> CreateGetUsersQuery(string accessToken)
        {
            _logger.LogDebug("Find out what roles user can read");

            var accessRoles = _permissionResolver.WhichRolesUserHasAccessTo(accessToken).ToList();

            if (!accessRoles.Any())
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access denied"));
            }

            _logger.LogDebug("Searching all available users");

            var query = _dBContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.University)
                .Where(x => x.UserRoles.Any(xx => accessRoles.Contains(xx.Role.Role)));

            var userHighestRole = _tokenDecoder.GetHighestUserRole(accessToken);

            if (userHighestRole == RolesConstants.UniversityAdmin ||
                userHighestRole == RolesConstants.Teacher ||
                userHighestRole == RolesConstants.Student)
            {
                _logger.LogDebug("Including user university");

                var tenantId = _tokenDecoder.GetUserTenantId(accessToken);

                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Unable to find user tenant"));
                }

                query = query.Where(x => x.TenantId == tenantId);
            }

            return query;
        }

        private string CreateToken(UserEntity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.EmailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                new Claim("id", user.Id.ToString()),
                new Claim("tenantId", user.TenantId ?? "")
            };
            claims.AddRange(user.UserRoles.Select(x => new Claim("role", x.Role.Role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpirationDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
