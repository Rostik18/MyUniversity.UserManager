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
using MyUniversity.UserManager.Services.Helpers;
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
                .FirstOrDefaultAsync(x => x.EmailAddress == email && x.IsSoftDeleted == false);

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

            var query = CreateGetUsersQuery(accessToken).AsNoTracking();

            _logger.LogDebug("Executing db query");

            var users = await query.ToListAsync();

            _logger.LogDebug($"Were found {users.Count} users");

            return users.Select(_mapper.Map<UserModel>);
        }

        public async Task<UserModel> GetUserByIdAsync(int id, string accessToken)
        {
            _logger.LogDebug("Preparing db query");

            var query = CreateGetUsersQuery(accessToken).AsNoTracking();

            _logger.LogDebug("Executing db query");

            var user = await query.FirstOrDefaultAsync(x => x.Id == id);

            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User with id {id} not found"));
            }

            return _mapper.Map<UserModel>(user);
        }

        public async Task<UserModel> UpdateUserAsync(UpdateUserModel updateModel, string accessToken)
        {
            _logger.LogDebug("Taking highest user role");

            var highestRole = _tokenDecoder.GetHighestUserRole(accessToken);

            if (highestRole == RolesConstants.Student &&
                _tokenDecoder.GetUserId(accessToken) != updateModel.Id)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Student cannot update another student"));
            }

            _logger.LogDebug("Creating query");

            var userQuery = CreateGetUsersQuery(accessToken);

            _logger.LogDebug("Executing query");

            var user = await userQuery.FirstOrDefaultAsync(x => x.Id == updateModel.Id);

            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User with id {updateModel.Id} not found"));
            }

            _logger.LogDebug("Updating user");

            if (!string.IsNullOrEmpty(updateModel.FirstName))
            {
                user.FirstName = updateModel.FirstName;
            }
            if (!string.IsNullOrEmpty(updateModel.LastName))
            {
                user.LastName = updateModel.LastName;
            }
            if (!string.IsNullOrEmpty(updateModel.PhoneNumber))
            {
                user.PhoneNumber = updateModel.PhoneNumber;
            }
            if (!string.IsNullOrEmpty(updateModel.EmailAddress))
            {
                if (await _dBContext.Users.AnyAsync(x => x.EmailAddress == updateModel.EmailAddress))
                {
                    throw new RpcException(new Status(StatusCode.AlreadyExists, $"User with email {updateModel.EmailAddress} already exist"));
                }

                user.EmailAddress = updateModel.EmailAddress;
            }
            if (!string.IsNullOrEmpty(updateModel.Password))
            {
                PasswordHashHelper.CreatePasswordHash(updateModel.Password, out var hash, out var salt);

                user.PasswordHash = hash;
                user.PasswordSalt = salt;
            }
            if (!string.IsNullOrEmpty(updateModel.UniversityId))
            {
                if (highestRole != RolesConstants.SuperAdmin)
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "You cannot change user university"));
                }

                var usiversity = await _dBContext.Universities.FirstOrDefaultAsync(x => x.TenantId == updateModel.UniversityId);

                if (usiversity is null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"University with id {updateModel.UniversityId} not found"));
                }

                user.TenantId = updateModel.UniversityId;
            }
            if (updateModel.Roles.Any())
            {
                var availableRoles = _permissionResolver.WhichRolesUserHasAccessTo(accessToken);

                var roles = await _dBContext.Roles.Where(x => availableRoles.Contains(x.Role) && updateModel.Roles.Contains(x.Id)).ToListAsync();

                if (updateModel.Roles.Count() != roles.Count)
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "You cannot change user roles"));
                }

                user.UserRoles.RemoveAll(x => true);
                user.UserRoles.AddRange(roles.Select(x => new UserRoleEntity { RoleId = x.Id }));
            }

            var updatedUser = _dBContext.Users.Update(user);
            await _dBContext.SaveChangesAsync();

            return _mapper.Map<UserModel>(updatedUser.Entity);
        }

        public async Task<bool> SoftDeleteUserAsync(int id, string accessToken)
        {
            var user = await GetUserForDeletingAsync(id, accessToken, false);

            _logger.LogDebug("Validating user permission");

            if (!_permissionResolver.CanSoftDeleteUserWithHighestRole(UserHelpers.GetHighestRole(user), accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "You do not have permissions to delete this user"));
            }

            _logger.LogDebug($"Soft deleting user with id {id}");

            user.IsSoftDeleted = true;

            var updatedUser = _dBContext.Users.Update(user);
            await _dBContext.SaveChangesAsync();

            return updatedUser != null;
        }

        public async Task<bool> HardDeleteUserAsync(int id, string accessToken)
        {
            var user = await GetUserForDeletingAsync(id, accessToken, true);

            _logger.LogDebug("Validating user permission");

            if (!_permissionResolver.CanHardDeleteUserWithHighestRole(UserHelpers.GetHighestRole(user), accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "You do not have permissions to delete this user"));
            }

            _logger.LogDebug($"Hard deleting user with id {id}");

            var updatedUser = _dBContext.Users.Remove(user);
            await _dBContext.SaveChangesAsync();

            return updatedUser != null;
        }

        private async Task<UserEntity> GetUserForDeletingAsync(int id, string accessToken, bool withDeleted)
        {
            _logger.LogDebug("Preparing db query");

            var query = CreateGetUsersQuery(accessToken, withDeleted);

            _logger.LogDebug("Executing query");

            var user = await query.FirstOrDefaultAsync(x => x.Id == id);

            if (user is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"User with id {id} not found"));
            }

            return user;
        }

        private IQueryable<UserEntity> CreateGetUsersQuery(string accessToken, bool withDeleted = false)
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
                .Where(x => x.UserRoles.Any(xx => accessRoles.Contains(xx.Role.Role)) &&
                            x.IsSoftDeleted == withDeleted);

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
