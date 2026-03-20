using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

using AutoMapper;

namespace teamseven.EzExam.Services.Services.TextBookService
{
    public class TextBookService : ITextBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TextBookService> _logger;
        private readonly IMapper _mapper;

        public TextBookService(IUnitOfWork unitOfWork, ILogger<TextBookService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TextBookDataResponse>> GetAllTextBookAsync()
        {
            var textbooks = await _unitOfWork.TextBookRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TextBookDataResponse>>(textbooks);
        }
        public async Task<List<TextBookDataResponse>> GetAsync(int? gradeId = null, int? subjectId = null)
        {
            var items = await _unitOfWork.TextBookRepository.GetAllAsync(gradeId, subjectId);
            return _mapper.Map<List<TextBookDataResponse>>(items);
        }

        public async Task<TextBookDataResponse> GetTextBookByIdAsync(int id)
        {
            var tb = await _unitOfWork.TextBookRepository.GetByIdAsync(id);
            if (tb == null)
                throw new NotFoundException($"Textbook with ID {id} not found.");

            return _mapper.Map<TextBookDataResponse>(tb);
        }

        public async Task CreateTextBookAsync(CreateTextBookRequest request)
        {
            var textbook = _mapper.Map<TextBook>(request);
            textbook.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.TextBookRepository.CreateAsync(textbook);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task UpdateTextBookAsync(TextBookDataRequest request)
        {
            var existing = await _unitOfWork.TextBookRepository.GetByIdAsync(request.Id);
            if (existing == null)
                throw new NotFoundException($"Textbook with ID {request.Id} not found.");

            existing.Name = request.Name;
            existing.GradeId = request.GradeId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TextBookRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task DeleteTextBookAsync(int id)
        {
            var textbook = await _unitOfWork.TextBookRepository.GetByIdAsync(id);
            if (textbook == null)
                throw new NotFoundException($"Textbook with ID {id} not found.");

            await _unitOfWork.TextBookRepository.RemoveAsync(textbook);
            await _unitOfWork.SaveChangesWithTransactionAsync();

        }
    }
}
