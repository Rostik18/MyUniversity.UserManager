using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MyUniversity.UserManager.Models.Constants;
using MyUniversity.UserManager.Models.University;
using MyUniversity.UserManager.Services;
using MyUniversity.UserManager.University;

namespace MyUniversity.UserManager.Controllers
{
    public class UniversityController : University.University.UniversityBase
    {
        private readonly ILogger<UniversityController> _logger;
        private readonly IUniversityService _universityService;
        private readonly IMapper _mapper;

        public UniversityController(
            ILogger<UniversityController> logger,
            IUniversityService universityService,
            IMapper mapper)
        {
            _logger = logger;
            _universityService = universityService;
            _mapper = mapper;
        }

        public override async Task<UniversityModelReply> CreateUniversity(CreateUniversityRequest request, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue(HeaderKeys.AccessToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized action"));
            }

            _logger.LogDebug("Mapping university create model");

            var createModel = _mapper.Map<CreateUniversityModel>(request);

            _logger.LogDebug("Creating new university");

            var createdModel = await _universityService.CreateUniversityAsync(createModel, accessToken);

            _logger.LogDebug("Sending creation response");

            return _mapper.Map<UniversityModelReply>(createdModel);
        }

        public override async Task<GetUniversitiesReply> GetUniversities(GetUniversitiesRequest request, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue(HeaderKeys.AccessToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized action"));
            }

            _logger.LogDebug("Creating new university");

            var allAvailableUniversities = await _universityService.GetAllUniversitiesAsync(accessToken);

            var replyModels = allAvailableUniversities.Select(_mapper.Map<UniversityModelReply>);

            _logger.LogDebug("Sending creation response");

            return new GetUniversitiesReply
            {
                Universities = { replyModels }
            };
        }

        public override async Task<UniversityModelReply> UpdateUniversity(UpdateUniversityRequest request, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue(HeaderKeys.AccessToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized action"));
            }

            _logger.LogDebug("Mapping university update model");

            var updateModel = _mapper.Map<UpdateUniversityModel>(request);

            _logger.LogDebug("Updating university");

            var updatedModel = await _universityService.UpdateUniversityAsync(updateModel, accessToken);

            _logger.LogDebug("Sending update response");

            return _mapper.Map<UniversityModelReply>(updatedModel);
        }

        public override async Task<DeleteUniversityReply> DeleteUniversity(DeleteUniversityRequest request, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue(HeaderKeys.AccessToken);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Unauthorized action"));
            }

            _logger.LogDebug("Deleting university");

            var success = await _universityService.DeleteUniversityAsync(request.Id, accessToken);

            _logger.LogDebug("Sending delete response");

            return new DeleteUniversityReply
            {
                Success = success
            };
        }
    }
}
