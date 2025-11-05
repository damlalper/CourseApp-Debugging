using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.LessonDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseApp.ServiceLayer.Concrete;

public class LessonsManager : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LessonsManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<IDataResult<IEnumerable<GetAllLessonDto>>> GetAllAsync(bool track = true)
    {
        var lessonList = await _unitOfWork.Lessons.GetAll(track).ToListAsync();
        var lessonListMapping = _mapper.Map<IEnumerable<GetAllLessonDto>>(lessonList);
        if (!lessonList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllLessonDto>>(null, ConstantsMessages.LessonListFailedMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllLessonDto>>(lessonListMapping, ConstantsMessages.LessonListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdLessonDto>> GetByIdAsync(string id, bool track = true)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdLessonDto>(null, "Invalid ID");
        }

        var hasLesson = await _unitOfWork.Lessons.GetByIdAsync(id, track);
        if (hasLesson == null)
        {
            return new ErrorDataResult<GetByIdLessonDto>(null, ConstantsMessages.LessonGetByIdFailedMessage);
        }

        var hasLessonMapping = _mapper.Map<GetByIdLessonDto>(hasLesson);
        return new SuccessDataResult<GetByIdLessonDto>(hasLessonMapping, ConstantsMessages.LessonGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateLessonDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var createdLesson = _mapper.Map<Lesson>(entity);
        if (createdLesson == null)
        {
            return new ErrorResult("Mapping failed");
        }

        await _unitOfWork.Lessons.CreateAsync(createdLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.LessonCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteLessonDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Lesson data cannot be null.");
        }

        var lessonToDelete = await _unitOfWork.Lessons.GetByIdAsync(entity.Id);
        if (lessonToDelete == null)
        {
            return new ErrorResult(ConstantsMessages.LessonGetByIdFailedMessage);
        }

        _unitOfWork.Lessons.Remove(lessonToDelete);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.LessonDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateLessonDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Entity or Title cannot be null");
        }

        var existingLesson = await _unitOfWork.Lessons.GetByIdAsync(entity.Id, true);
        if (existingLesson == null)
        {
            return new ErrorResult(ConstantsMessages.LessonGetByIdFailedMessage);
        }

        _mapper.Map(entity, existingLesson);

        _unitOfWork.Lessons.Update(existingLesson);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonUpdateSuccessMessage);
        }
        
        return new ErrorResult(ConstantsMessages.LessonUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllLessonDetailDto>>> GetAllLessonDetailAsync(bool track = true)
    {
        var lessonList = await _unitOfWork.Lessons.GetAllLessonDetails(track).ToListAsync();
        var lessonsListMapping = _mapper.Map<IEnumerable<GetAllLessonDetailDto>>(lessonList);

        if (lessonsListMapping == null || !lessonsListMapping.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllLessonDetailDto>>(null, ConstantsMessages.LessonListFailedMessage);
        }

        return new SuccessDataResult<IEnumerable<GetAllLessonDetailDto>>(lessonsListMapping, ConstantsMessages.LessonListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdLessonDetailDto>> GetByIdLessonDetailAsync(string id, bool track = true)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdLessonDetailsAsync(id, track);
        var lessonMapping = _mapper.Map<GetByIdLessonDetailDto>(lesson);
        return new SuccessDataResult<GetByIdLessonDetailDto>(lessonMapping);
    }
}
