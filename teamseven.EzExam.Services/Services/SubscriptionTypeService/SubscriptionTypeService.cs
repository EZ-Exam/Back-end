using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Services.Services.SubscriptionTypeService
{
    public class SubscriptionTypeService : ISubscriptionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SubscriptionTypeService> _logger;

        public SubscriptionTypeService(IUnitOfWork unitOfWork, ILogger<SubscriptionTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<SubscriptionTypeResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.SubscriptionTypeRepository.GetAllAsync();
            return list.Select(x => new SubscriptionTypeResponse
            {
                Id = x.Id,
                SubscriptionCode = x.SubscriptionCode,
                SubscriptionName = x.SubscriptionName,
                SubscriptionPrice = x.SubscriptionPrice ?? 0,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).ToList();
        }

        public async Task<SubscriptionTypeResponse> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
            if (item == null) throw new KeyNotFoundException("Not found");

            return new SubscriptionTypeResponse
            {
                Id = item.Id,
                SubscriptionCode = item.SubscriptionCode,
                SubscriptionName = item.SubscriptionName,
                SubscriptionPrice = item.SubscriptionPrice ?? 0,
                Description = item.Description,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }

        public async Task AddAsync(SubscriptionTypeRequest request)
        {
            var entity = new SubscriptionType
            {
                SubscriptionCode = request.SubscriptionCode,
                SubscriptionName = request.SubscriptionName,
                SubscriptionPrice = request.SubscriptionPrice,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.SubscriptionTypeRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task UpdateAsync(int id, SubscriptionTypeRequest request)
        {
            var entity = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
            if (entity == null) throw new KeyNotFoundException("Not found");

            entity.SubscriptionCode = request.SubscriptionCode;
            entity.SubscriptionName = request.SubscriptionName;
            entity.SubscriptionPrice = request.SubscriptionPrice;
            entity.Description = request.Description;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SubscriptionTypeRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.SubscriptionTypeRepository.GetByIdAsync(id);
            if (entity != null)
            {
                await _unitOfWork.SubscriptionTypeRepository.RemoveAsync(entity);
                await _unitOfWork.SaveChangesWithTransactionAsync();
            }
        }
    }
}
