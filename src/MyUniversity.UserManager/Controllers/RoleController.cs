using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MyUniversity.UserManager.Models.Constants;
using MyUniversity.UserManager.Services;

namespace MyUniversity.UserManager.Controllers
{
    public class RoleController : Role.RoleBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleController(
            ILogger<RoleController> logger,
            IRoleService roleService,
            IMapper mapper)
        {
            _logger = logger;
            _roleService = roleService;
            _mapper = mapper;
        }

        public override async Task<RolesReply> GetRoles(RoleRequest request, ServerCallContext context)
        {
            _logger.LogDebug("Checking the user access token");

            var accessToken = context.RequestHeaders.GetValue(HeaderKeys.AccessToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized action"));
            }

            _logger.LogDebug("Getting roles");

            var roles = await _roleService.GetRolesAsync(accessToken);

            var rolesReply = roles.Select(_mapper.Map<RoleReply>);

            _logger.LogDebug("Sending get roles response");

            return new RolesReply
            {
                Roles = { rolesReply }
            };
        }
    }
}
