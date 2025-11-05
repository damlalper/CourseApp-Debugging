using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.InstructorDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseApp.ServiceLayer.Concrete;

public class InstructorManager : IInstructorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public InstructorManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllInstructorDto>>> GetAllAsync(bool track = true)
    {
        var instructorList = await _unitOfWork.Instructors.GetAll(track).ToListAsync();
        var instructorListMapping = _mapper.Map<IEnumerable<GetAllInstructorDto>>(instructorList);
        if (!instructorList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllInstructorDto>>(null, ConstantsMessages.InstructorListFailedMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllInstructorDto>>(instructorListMapping, ConstantsMessages.InstructorListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdInstructorDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdInstructorDto>(null, "Invalid ID");
        }

        var hasInstructor = await _unitOfWork.Instructors.GetByIdAsync(id, track);
        if (hasInstructor == null)
        {
            return new ErrorDataResult<GetByIdInstructorDto>(null, ConstantsMessages.InstructorGetByIdFailedMessage);
        }

        var hasInstructorMapping = _mapper.Map<GetByIdInstructorDto>(hasInstructor);
        if (hasInstructorMapping == null)
        {
            return new ErrorDataResult<GetByIdInstructorDto>(null, "Mapping failed");
        }

        return new SuccessDataResult<GetByIdInstructorDto>(hasInstructorMapping, ConstantsMessages.InstructorGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreatedInstructorDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var createdInstructor = _mapper.Map<Instructor>(entity);
        if (createdInstructor == null)
        {
            return new ErrorResult("Mapping failed");
        }

        await _unitOfWork.Instructors.CreateAsync(createdInstructor);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.InstructorCreateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.InstructorCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeletedInstructorDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Instructor data cannot be null.");
        }

        var instructorToDelete = await _unitOfWork.Instructors.GetByIdAsync(entity.Id);
        if (instructorToDelete == null)
        {
            return new ErrorResult(ConstantsMessages.InstructorGetByIdFailedMessage);
        }

        _unitOfWork.Instructors.Remove(instructorToDelete);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.InstructorDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.InstructorDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdatedInstructorDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var existingInstructor = await _unitOfWork.Instructors.GetByIdAsync(entity.Id, true);
        if (existingInstructor == null)
        {
            return new ErrorResult(ConstantsMessages.InstructorGetByIdFailedMessage);
        }

        _mapper.Map(entity, existingInstructor);

        _unitOfWork.Instructors.Update(existingInstructor);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.InstructorUpdateSuccessMessage);
        }
        
        return new ErrorResult(ConstantsMessages.InstructorUpdateFailedMessage);
    }
}
