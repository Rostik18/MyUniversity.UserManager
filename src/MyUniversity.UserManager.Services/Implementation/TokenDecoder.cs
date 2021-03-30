using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Services.Settings;

namespace MyUniversity.UserManager.Services.Implementation
{
    public class TokenDecoder : ITokenDecoder
    {
        private readonly JwtSettings _jwtSettings;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public TokenDecoder(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public IEnumerable<string> GetUserRoles(string accessToken)
        {
            accessToken = accessToken.Replace("bearer ", "");

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            var claims = _jwtSecurityTokenHandler.ValidateToken(accessToken, validations, out _);

            var roles = claims.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(x => x.Value.Split(',').ToList()).FirstOrDefault();

            return roles;
        }

        public string GetHighestUserRole(string accessToken)
        {
            var roles = GetUserRoles(accessToken).ToList();

            if (roles.Contains(RolesConstants.SuperAdmin)) return RolesConstants.SuperAdmin;
            if (roles.Contains(RolesConstants.Service)) return RolesConstants.Service;
            if (roles.Contains(RolesConstants.UniversityAdmin)) return RolesConstants.UniversityAdmin;
            if (roles.Contains(RolesConstants.Teacher)) return RolesConstants.Teacher;
            if (roles.Contains(RolesConstants.Student)) return RolesConstants.Student;

            return null;
        }
    }
}
