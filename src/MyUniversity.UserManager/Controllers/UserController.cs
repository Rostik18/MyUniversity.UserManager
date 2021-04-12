using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MyUniversity.UserManager.Models.Constants;
using MyUniversity.UserManager.Models.User;
using MyUniversity.UserManager.Services;
using MyUniversity.UserManager.User;

namespace MyUniversity.UserManager.Controllers
{
    public class UserController : User.User.UserBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        public override async Task<RegistrationReply> RegisterUser(RegistrationRequest request, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue(HeaderKeys.AccessToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized action"));
            }

            _logger.LogDebug("Mapping user registration model");

            var registrationModel = _mapper.Map<RegisterUserModel>(request);

            _logger.LogDebug("Registering new user");

            var registeredModel = await _userService.RegisterUserAsync(registrationModel, accessToken);

            _logger.LogDebug("Sending registration success response");

            return new RegistrationReply
            {
                RegistrationSuccess = registeredModel is not null
            };
        }

        public override async Task<LoginReply> LoginUser(LoginRequest request, ServerCallContext context)
        {
            _logger.LogDebug($"User with email {request.EmailAddress} logging into the system");

            var userToken = await _userService.LoginUserAsync(request.EmailAddress, request.Password);

            _logger.LogDebug("Sending login response");

            return new LoginReply
            {
                EmailAddress = request.EmailAddress,
                Token = userToken
            };
        }

        public override async Task<GetAllUsersReply> GetAllUsers(GetUsersRequest request, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue(HeaderKeys.AccessToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized action"));
            }

            _logger.LogDebug("Getting all available users");

            var availableUsers = await _userService.GetAllUsersAsync(accessToken);

            var users = availableUsers.Select(_mapper.Map<UserModelReply>);

            _logger.LogDebug("Sending get all users response");

            return new GetAllUsersReply
            {
                Users = { users }
            };
        }

        public override async Task<UserModelReply> GetUserById(GetUserRequest request, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue(HeaderKeys.AccessToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized action"));
            }

            _logger.LogDebug($"Getting user by id {request.Id}");

            var availableUser = await _userService.GetUserByIdAsync(request.Id, accessToken);

            var user = _mapper.Map<UserModelReply>(availableUser);

            _logger.LogDebug("Sending get user by id response");

            return user;
        }
    }
}
