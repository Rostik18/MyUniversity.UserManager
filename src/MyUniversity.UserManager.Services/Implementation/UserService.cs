using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyUniversity.UserManager.Models.CustomExceptions;
using MyUniversity.UserManager.Models.User;
using MyUniversity.UserManager.Repository.DbContext;
using MyUniversity.UserManager.Repository.Entities.User;
using MyUniversity.UserManager.Repository.Helpers;
using MyUniversity.UserManager.Services.Settings;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyUniversity.UserManager.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UMDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public UserService(
            ILogger<UserService> logger,
            UMDBContext dBContext,
            IMapper mapper,
            IOptions<JwtSettings> jwtSettings)
        {
            _logger = logger;
            _dBContext = dBContext;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<UserModel> RegisterUserAsync(RegisterUserModel userModel)
        {
            var userEntity = await _dBContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == userModel.EmailAddress);

            if (userEntity is not null)
            {
                throw new BadArgumentException($"User with email {userModel.EmailAddress} already exists");
            }

            var roles = await _dBContext.Roles.ToListAsync();

            var newUserRoles = roles.Where(e => userModel.Roles.Any(role => role == e.Role));

            // to do: user roles cteation based on creator permissions

            if (!newUserRoles.Any())
            {
                throw new BadArgumentException("Unable to create user without role");
            }

            PasswordHashHelper.CreatePasswordHash(userModel.Password, out var hash, out var salt);

            userEntity = new UserEntity
            {
                FirstName = userModel.LastName,
                LastName = userModel.LastName,
                EmailAddress = userModel.EmailAddress,
                PhoneNumber = userModel.PhoneNumber,
                TenantId = userModel.TenantId,
                UserRoles = newUserRoles.Select(x => new UserRoleEntity { RoleId = x.Id }),
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
            var user = await _dBContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.EmailAddress == email);

            if (user is null)
            {
                throw new AccessForbiddenException("Invalid email or password");
            }

            var isUserVerified = PasswordHashHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt);

            if (!isUserVerified)
            {
                throw new AccessForbiddenException("Invalid email or password");
            }

            return CreateToken(user);
        }

        public async Task<UserModel> GetUserAsync(int id)
        {
            var userEntity = await _dBContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (userEntity is null)
            {
                throw new ItemNotFoundException($"User with id {id} not found");
            }

            return _mapper.Map<UserModel>(userEntity);
        }

        public async Task<UserModel> GetUserAsync(string email)
        {
            var userEntity = await _dBContext.Users.FirstOrDefaultAsync(x => x.EmailAddress == email);

            if (userEntity is null)
            {
                throw new ItemNotFoundException($"User with email {email} not found");
            }

            return _mapper.Map<UserModel>(userEntity);
        }

        private string CreateToken(UserEntity user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.EmailAddress),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                    new Claim("id", user.Id.ToString()),
                    new Claim("tenantId", user.TenantId),
                    new Claim("roles", string.Join(',', user.UserRoles.Select(x => x.Role.Role))),
                }),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpirationDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
