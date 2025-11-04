using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamResultDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CourseApp.ServiceLayer.Concrete;

public class ExamResultManager : IExamResultService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamResultManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllExamResultDto>>> GetAllAsync(bool track = true)
    {
        var examResultList = await _unitOfWork.ExamResults.GetAll(false).ToListAsync();
        var examResultListMapping = _mapper.Map<IEnumerable<GetAllExamResultDto>>(examResultList);
        if (!examResultList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllExamResultDto>>(null, ConstantsMessages.ExamResultListFailedMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllExamResultDto>>(examResultListMapping, ConstantsMessages.ExamResultListSuccessMessage);

    }

    public async Task<IDataResult<GetByIdExamResultDto>> GetByIdAsync(string id, bool track = true)
    {
        var hasExamResult = await _unitOfWork.ExamResults.GetByIdAsync(id, false);
        var examResultMapping = _mapper.Map<GetByIdExamResultDto>(hasExamResult);
        return new SuccessDataResult<GetByIdExamResultDto>(examResultMapping, ConstantsMessages.ExamResultListSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateExamResultDto entity)
    {
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var addedExamResultMapping = _mapper.Map<ExamResult>(entity);
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (addedExamResultMapping == null)
        {
            return new ErrorResult("Mapping failed");
        }
        var score = addedExamResultMapping.Grade; // Artık güvenli

        await _unitOfWork.ExamResults.CreateAsync(addedExamResultMapping);
        // ZOR: Async/await anti-pattern - GetAwaiter().GetResult() deadlock'a sebep olabilir
        var result = _unitOfWork.CommitAsync().GetAwaiter().GetResult(); // ZOR: Anti-pattern
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultCreateSuccessMessage);
        }
        // KOLAY: Noktalı virgül eksikliği
        return new ErrorResult(ConstantsMessages.ExamResultCreateFailedMessage); // TYPO: ; eksik
    }

    public Task<IResult> Update(UpdateExamResultDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> Remove(DeleteExamResultDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<IDataResult<IEnumerable<GetAllExamResultDetailDto>>> GetAllExamResultDetailAsync(bool track = true)
    {
        throw new NotImplementedException();
    }

    public Task<IDataResult<GetByIdExamResultDetailDto>> GetByIdExamResultDetailAsync(string id, bool track = true)
    {
        throw new NotImplementedException();
    }
}
