using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseApp.ServiceLayer.Concrete;

public class ExamManager : IExamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllExamDto>>> GetAllAsync(bool track = true)
    {
        var examList = await _unitOfWork.Exams.GetAll(track).ToListAsync();
        var examListMapping = _mapper.Map<IEnumerable<GetAllExamDto>>(examList);

        if (examListMapping == null || !examListMapping.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllExamDto>>(null, ConstantsMessages.ExamListFailedMessage);
        }

        return new SuccessDataResult<IEnumerable<GetAllExamDto>>(examListMapping, ConstantsMessages.ExamListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdExamDto>> GetByIdAsync(string id, bool track = true)
    {
        var hasExam = await _unitOfWork.Exams.GetByIdAsync(id, track);
        var examResultMapping = _mapper.Map<GetByIdExamDto>(hasExam);
        return new SuccessDataResult<GetByIdExamDto>(examResultMapping, ConstantsMessages.ExamGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateExamDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var addedExamMapping = _mapper.Map<Exam>(entity);
        if (addedExamMapping == null)
        {
            return new ErrorResult("Mapping failed");
        }

        await _unitOfWork.Exams.CreateAsync(addedExamMapping);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamCreateSuccessMessage);
        }
        
        return new ErrorResult(ConstantsMessages.ExamCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteExamDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Exam data cannot be null.");
        }

        var examToDelete = await _unitOfWork.Exams.GetByIdAsync(entity.Id);
        if (examToDelete == null)
        {
            return new ErrorResult(ConstantsMessages.ExamGetByIdFailedMessage);
        }

        _unitOfWork.Exams.Remove(examToDelete);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateExamDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Exam data cannot be null.");
        }

        var existingExam = await _unitOfWork.Exams.GetByIdAsync(entity.Id, true);
        if (existingExam == null)
        {
            return new ErrorResult(ConstantsMessages.ExamGetByIdFailedMessage);
        }

        _mapper.Map(entity, existingExam);

        _unitOfWork.Exams.Update(existingExam);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamUpdateFailedMessage);
    }
}
