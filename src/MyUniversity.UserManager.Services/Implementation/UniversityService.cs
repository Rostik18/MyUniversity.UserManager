using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyUniversity.UserManager.Models.University;
using MyUniversity.UserManager.Repository.DbContext;
using MyUniversity.UserManager.Repository.Entities.University;

namespace MyUniversity.UserManager.Services.Implementation
{
    public class UniversityService : IUniversityService
    {
        private readonly ILogger<UniversityService> _logger;
        private readonly UMDBContext _dBContext;
        private readonly IMapper _mapper;
        private readonly ITokenDecoder _tokenDecoder;
        private readonly IPermissionResolver _permissionResolver;

        public UniversityService(
            ILogger<UniversityService> logger,
            UMDBContext dBContext,
            IMapper mapper,
            ITokenDecoder tokenDecoder,
            IPermissionResolver permissionResolver)
        {
            _logger = logger;
            _dBContext = dBContext;
            _mapper = mapper;
            _tokenDecoder = tokenDecoder;
            _permissionResolver = permissionResolver;
        }

        public async Task<UniversityModel> CreateUniversityAsync(CreateUniversityModel model, string accessToken)
        {
            _logger.LogDebug("Validating user access");

            if (!_permissionResolver.CanUserCreateUniversity(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access denied"));
            }

            _logger.LogDebug("Validating if similar university exists");

            var isSimilarExist = await _dBContext.Universities.AnyAsync(x =>
                x.Name == model.Name || x.Address == model.Address || x.EmailAddress == model.EmailAddress || x.PhoneNumber == model.PhoneNumber);

            if (isSimilarExist)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Similar university already exists"));
            }

            _logger.LogDebug("Creating university");

            var newUniversity = new UniversityEntity
            {
                TenantId = Guid.NewGuid().ToString(),
                Name = model.Name,
                Address = model.Address,
                EmailAddress = model.EmailAddress,
                PhoneNumber = model.PhoneNumber
            };

            var createdUniversity = await _dBContext.Universities.AddAsync(newUniversity);
            await _dBContext.SaveChangesAsync();

            return _mapper.Map<UniversityModel>(createdUniversity.Entity);
        }

        public async Task<IEnumerable<UniversityModel>> GetAllUniversitiesAsync(string accessToken)
        {
            _logger.LogDebug("Creating db query based on user access");

            var query = _dBContext.Universities.AsNoTracking();

            if (!_permissionResolver.CanUserReadAllUniversities(accessToken))
            {
                var userTenantId = _tokenDecoder.GetUserTenantId(accessToken);
                query = query.Where(x => x.TenantId == userTenantId);
            }

            _logger.LogDebug("Executing query");

            var universities = await query.ToListAsync();

            return universities.Select(_mapper.Map<UniversityModel>);
        }

        public async Task<UniversityModel> UpdateUniversityAsync(UpdateUniversityModel model, string accessToken)
        {
            _logger.LogDebug("Validating user access");

            if (!_permissionResolver.CanUserUpdateUniversity(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access denied"));
            }

            _logger.LogDebug("Validating if university exists and can be updated");

            var university = await _dBContext.Universities.FirstOrDefaultAsync(x => x.TenantId == model.Id);

            if (university is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"University with id {model.Id} not found"));
            }

            var isSimilarExist = await _dBContext.Universities.AnyAsync(x =>
                x.Name == model.Name || x.Address == model.Address || x.EmailAddress == model.EmailAddress || x.PhoneNumber == model.PhoneNumber);

            if (isSimilarExist)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Similar university already exists"));
            }

            _logger.LogDebug("Updating university");

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                university.Name = model.Name;
            }
            if (!string.IsNullOrWhiteSpace(model.Address))
            {
                university.Address = model.Address;
            }
            if (!string.IsNullOrWhiteSpace(model.EmailAddress))
            {
                university.EmailAddress = model.EmailAddress;
            }
            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                university.PhoneNumber = model.PhoneNumber;
            }

            var updatedUniversity = _dBContext.Universities.Update(university);
            await _dBContext.SaveChangesAsync();

            return _mapper.Map<UniversityModel>(updatedUniversity.Entity);
        }

        public async Task<bool> DeleteUniversityAsync(string id, string accessToken)
        {
            _logger.LogDebug("Validating user access");

            if (!_permissionResolver.CanUserDeleteUniversity(accessToken))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access denied"));
            }

            _logger.LogDebug("Validating if university exists");

            var university = await _dBContext.Universities.FirstOrDefaultAsync(x => x.TenantId == id);

            if (university is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"University with id {id} not found"));
            }

            _logger.LogDebug("Deleting university");

            _dBContext.Universities.Remove(university);
            await _dBContext.SaveChangesAsync();

            return true;
        }
    }
}
