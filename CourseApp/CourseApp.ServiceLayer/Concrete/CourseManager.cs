using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.CourseDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseApp.ServiceLayer.Concrete;

public class CourseManager : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CourseManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllCourseDto>>> GetAllAsync(bool track = true)
    {
        var courseList = await _unitOfWork.Courses.GetAll(track).ToListAsync();
        var courseListMapping = _mapper.Map<IEnumerable<GetAllCourseDto>>(courseList);

        if (courseListMapping == null || !courseListMapping.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllCourseDto>>(null, ConstantsMessages.CourseListFailedMessage);
        }

        return new SuccessDataResult<IEnumerable<GetAllCourseDto>>(courseListMapping, ConstantsMessages.CourseListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdCourseDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdCourseDto>(null, "Invalid ID");
        }

        var hasCourse = await _unitOfWork.Courses.GetByIdAsync(id, track);

        if (hasCourse == null)
        {
            return new ErrorDataResult<GetByIdCourseDto>(null, ConstantsMessages.CourseGetByIdFailedMessage);
        }

        var courseMapping = _mapper.Map<GetByIdCourseDto>(hasCourse);
        return new SuccessDataResult<GetByIdCourseDto>(courseMapping, ConstantsMessages.CourseGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateCourseDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Course data cannot be null.");
        }

        var validationResult = await ValidateCourse(entity.CourseName, entity.StartDate, entity.EndDate, entity.InstructorID);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var createdCourse = _mapper.Map<Course>(entity);

        await _unitOfWork.Courses.CreateAsync(createdCourse);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.CourseCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.CourseCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteCourseDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Course data cannot be null.");
        }

        var courseToDelete = await _unitOfWork.Courses.GetByIdAsync(entity.Id);
        if (courseToDelete == null)
        {
            return new ErrorResult(ConstantsMessages.CourseGetByIdFailedMessage);
        }

        _unitOfWork.Courses.Remove(courseToDelete);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.CourseDeleteSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.CourseDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateCourseDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Course data cannot be null.");
        }

        var validationResult = await ValidateCourse(entity.CourseName, entity.StartDate, entity.EndDate, entity.InstructorID, entity.Id);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        var updatedCourse = await _unitOfWork.Courses.GetByIdAsync(entity.Id, true);
        if (updatedCourse == null)
        {
            return new ErrorResult(ConstantsMessages.CourseGetByIdFailedMessage);
        }

        _mapper.Map(entity, updatedCourse);

        _unitOfWork.Courses.Update(updatedCourse);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.CourseUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.CourseUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllCourseDetailDto>>> GetAllCourseDetail(bool track = true)
    {
        var courseListDetailList = await _unitOfWork.Courses.GetAllCourseDetail(track).ToListAsync();
        var courseDetailDtoList = _mapper.Map<IEnumerable<GetAllCourseDetailDto>>(courseListDetailList);

        if (courseDetailDtoList == null || !courseDetailDtoList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllCourseDetailDto>>(null, ConstantsMessages.CourseListFailedMessage);
        }

        return new SuccessDataResult<IEnumerable<GetAllCourseDetailDto>>(courseDetailDtoList, ConstantsMessages.CourseDetailsFetchedSuccessfully);
    }

    private async Task<IResult> ValidateCourse(string? courseName, DateTime startDate, DateTime endDate, string? instructorId, string? id = null)
    {
        var nameCheck = CourseNameIsNullOrEmpty(courseName);
        if (!nameCheck.IsSuccess) return nameCheck;

        var lengthCheck = CourseNameLengthCheck(courseName);
        if (!lengthCheck.IsSuccess) return lengthCheck;

        var dateCheck = CheckCourseDates(startDate, endDate);
        if (!dateCheck.IsSuccess) return dateCheck;

        if (!string.IsNullOrEmpty(instructorId))
        {
            var instructorExists = await _unitOfWork.Instructors.GetByIdAsync(instructorId);
            if (instructorExists == null)
            {
                return new ErrorResult("Instructor not found.");
            }
        }

        var uniqueCheck = await CourseNameUniqueCheck(courseName!, id);
        if (!uniqueCheck.IsSuccess) return uniqueCheck;

        return new SuccessResult();
    }

    private IResult CourseNameIsNullOrEmpty(string? courseName)
    {
        if (string.IsNullOrEmpty(courseName))
        {
            return new ErrorResult("Course Name cannot be empty.");
        }
        return new SuccessResult();
    }

    private async Task<IResult> CourseNameUniqueCheck(string courseName, string? id = null)
    {
        var query = _unitOfWork.Courses.GetAll(false);
        bool courseNameExists;

        if (id != null)
        {
            // For updates, check if the name exists on a DIFFERENT course
            courseNameExists = await query.AnyAsync(c => c.CourseName == courseName && c.ID != id);
        }
        else
        {
            // For creates, check if the name exists at all
            courseNameExists = await query.AnyAsync(c => c.CourseName == courseName);
        }
        
        if (courseNameExists)
        {
            return new ErrorResult("A course with this name already exists.");
        }
        return new SuccessResult();
    }

    private IResult CourseNameLengthCheck(string? courseName)
    {
        if (string.IsNullOrEmpty(courseName) || courseName.Length < 2 || courseName.Length > 50)
        {
            return new ErrorResult("Course Name must be between 2 and 50 characters.");
        }
        return new SuccessResult();
    }

    private IResult CheckCourseDates(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
        {
            return new ErrorResult("End date must be after the start date.");
        }
        return new SuccessResult();
    }
}
