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
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var addedExamResultMapping = _mapper.Map<ExamResult>(entity);
        if (addedExamResultMapping == null)
        {
            return new ErrorResult("Mapping failed");
        }

        await _unitOfWork.ExamResults.CreateAsync(addedExamResultMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultCreateSuccessMessage);
        }
        
        return new ErrorResult(ConstantsMessages.ExamResultCreateFailedMessage);
    }

    public async Task<IResult> Update(UpdateExamResultDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("ExamResult data cannot be null.");
        }

        var existingExamResult = await _unitOfWork.ExamResults.GetByIdAsync(entity.Id, true);
        if (existingExamResult == null)
        {
            return new ErrorResult(ConstantsMessages.ExamResultGetByIdFailedMessage);
        }

        _mapper.Map(entity, existingExamResult);

        _unitOfWork.ExamResults.Update(existingExamResult);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamResultUpdateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteExamResultDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("ExamResult data cannot be null.");
        }

        var examResultToDelete = await _unitOfWork.ExamResults.GetByIdAsync(entity.Id, true);
        if (examResultToDelete == null)
        { 
            return new ErrorResult(ConstantsMessages.ExamResultGetByIdFailedMessage);
        }

        _unitOfWork.ExamResults.Remove(examResultToDelete);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamResultDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamResultDeleteFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllExamResultDetailDto>>> GetAllExamResultDetailAsync(bool track = true)
    {
        var examResultList = await _unitOfWork.ExamResults.GetAllExamResultDetail(false).ToListAsync();
        var examResultListMapping = _mapper.Map<IEnumerable<GetAllExamResultDetailDto>>(examResultList);

        if (!examResultList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllExamResultDetailDto>>(null, ConstantsMessages.ExamResultListFailedMessage);
        }

        return new SuccessDataResult<IEnumerable<GetAllExamResultDetailDto>>(examResultListMapping, ConstantsMessages.ExamResultListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdExamResultDetailDto>> GetByIdExamResultDetailAsync(string id, bool track = true)
    {
        var examResult = await _unitOfWork.ExamResults.GetByIdExamResultDetailAsync(id, false);

        if (examResult == null)
        {
            return new ErrorDataResult<GetByIdExamResultDetailDto>(null, ConstantsMessages.ExamResultListFailedMessage);
        }

        var examResultMapping = _mapper.Map<GetByIdExamResultDetailDto>(examResult);
        return new SuccessDataResult<GetByIdExamResultDetailDto>(examResultMapping, ConstantsMessages.ExamResultListSuccessMessage);
    }
}
