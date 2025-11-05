using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.LessonDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

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
        var lessonList = await _unitOfWork.Lessons.GetAll(false).ToListAsync();
        var lessonListMapping = _mapper.Map<IEnumerable<GetAllLessonDto>>(lessonList);
        if (!lessonList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllLessonDto>>(null, ConstantsMessages.LessonListFailedMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllLessonDto>>(lessonListMapping, ConstantsMessages.LessonListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdLessonDto>> GetByIdAsync(string id, bool track = true)
    {
        // ORTA DÜZELTME: Null ve empty check eklendi
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdLessonDto>(null, "Invalid ID");
        }

        var hasLesson = await _unitOfWork.Lessons.GetByIdAsync(id, false);
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (hasLesson == null)
        {
            return new ErrorDataResult<GetByIdLessonDto>(null, ConstantsMessages.LessonGetByIdFailedMessage);
        }

        var hasLessonMapping = _mapper.Map<GetByIdLessonDto>(hasLesson);
        // ORTA DÜZELTME: Doğru mesaj kullanıldı
        return new SuccessDataResult<GetByIdLessonDto>(hasLessonMapping, ConstantsMessages.LessonGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateLessonDto entity)
    {
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var createdLesson = _mapper.Map<Lesson>(entity);
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (createdLesson == null)
        {
            return new ErrorResult("Mapping failed");
        }

        // ZOR DÜZELTME: Async/await anti-pattern düzeltildi
        await _unitOfWork.Lessons.CreateAsync(createdLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonCreateSuccessMessage);
        }

        // KOLAY: Noktalı virgül eksikliği
        return new ErrorResult(ConstantsMessages.LessonCreateFailedMessage); // TYPO: ; eksik
    }

    public async Task<IResult> Remove(DeleteLessonDto entity)
    {
        var deletedLesson = _mapper.Map<Lesson>(entity);
        _unitOfWork.Lessons.Remove(deletedLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.LessonDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateLessonDto entity)
    {
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (entity == null || string.IsNullOrEmpty(entity.Title))
        {
            return new ErrorResult("Entity or Title cannot be null");
        }

        var updatedLesson = _mapper.Map<Lesson>(entity);

        _unitOfWork.Lessons.Update(updatedLesson);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.LessonUpdateSuccessMessage);
        }
        // ORTA DÜZELTME: ErrorResult olarak değiştirildi
        return new ErrorResult(ConstantsMessages.LessonUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllLessonDetailDto>>> GetAllLessonDetailAsync(bool track = true)
    {
        // ZOR: N+1 Problemi - Include kullanılmamış, lazy loading aktif
        var lessonList = await _unitOfWork.Lessons.GetAllLessonDetails(false).ToListAsync();

        // ZOR: N+1 - Her lesson için Course ayrı sorgu ile çekiliyor (lesson.Course?.CourseName)
        var lessonsListMapping = _mapper.Map<IEnumerable<GetAllLessonDetailDto>>(lessonList);

        // ORTA DÜZELTME: Null ve empty kontrolü eklendi
        if (lessonsListMapping == null || !lessonsListMapping.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllLessonDetailDto>>(null, ConstantsMessages.LessonListFailedMessage);
        }

        return new SuccessDataResult<IEnumerable<GetAllLessonDetailDto>>(lessonsListMapping, ConstantsMessages.LessonListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdLessonDetailDto>> GetByIdLessonDetailAsync(string id, bool track = true)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdLessonDetailsAsync(id, false);
        var lessonMapping = _mapper.Map<GetByIdLessonDetailDto>(lesson);
        return new SuccessDataResult<GetByIdLessonDetailDto>(lessonMapping);
    }
}
