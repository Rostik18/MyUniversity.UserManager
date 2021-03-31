using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyUniversity.UserManager.Models.Roles;
using MyUniversity.UserManager.Repository.DbContext;

namespace MyUniversity.UserManager.Services.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly UMDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly ITokenDecoder _tokenDecoder;

        public RoleService(
            ILogger<RoleService> logger,
            UMDBContext dBContext,
            IMapper mapper,
            ITokenDecoder tokenDecoder)
        {
            _logger = logger;
            _dBContext = dBContext;
            _mapper = mapper;
            _tokenDecoder = tokenDecoder;
        }

        public async Task<IEnumerable<RoleModel>> GetRolesAsync(string accessToken)
        {
            _logger.LogDebug("Getting roles based on user access level");

            var highestRole = _tokenDecoder.GetHighestUserRole(accessToken);

            var allRoles = await _dBContext.Roles.OrderBy(x => x.Id).AsNoTracking().ToListAsync();

            return highestRole switch
            {
                RolesConstants.SuperAdmin => allRoles.Select(_mapper.Map<RoleModel>),
                RolesConstants.Service => allRoles.Select(_mapper.Map<RoleModel>),
                RolesConstants.UniversityAdmin => allRoles
                    .Where(x => x.Role != RolesConstants.SuperAdmin && x.Role != RolesConstants.Service)
                    .Select(_mapper.Map<RoleModel>),
                RolesConstants.Teacher => allRoles
                    .Where(x => x.Role == RolesConstants.Student)
                    .Select(_mapper.Map<RoleModel>),
                _ => throw new RpcException(new Status(StatusCode.PermissionDenied, "Access denied"))
            };
        }
    }
}
