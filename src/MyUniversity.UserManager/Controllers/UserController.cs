using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MyUniversity.UserManager.Models.User;

namespace MyUniversity.UserManager.Services
{
    public class UserController : User.UserBase
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
            var registrationModel = _mapper.Map<RegisterUserModel>(request);

            var registeredModel = await _userService.RegisterUserAsync(registrationModel);

            return new RegistrationReply
            {
                RegistrationSuccess = registeredModel is not null
            };
        }

        public override async Task<LoginReply> LoginUser(LoginRequest request, ServerCallContext context)
        {
            return new LoginReply
            {
                EmailAddress = request.EmailAddress,
                Token = await _userService.LoginUserAsync(request.EmailAddress, request.Password)
            };
        }
    }
}
