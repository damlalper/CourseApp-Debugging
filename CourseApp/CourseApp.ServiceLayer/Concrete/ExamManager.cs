using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.ExamDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

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
        // ZOR: Async/await anti-pattern - async metot içinde senkron ToList kullanımı
        var examList = _unitOfWork.Exams.GetAll(false).ToList(); // ZOR: ToListAsync kullanılmalıydı
        // KOLAY: Değişken adı typo - examtListMapping yerine examListMapping
        var examtListMapping = _mapper.Map<IEnumerable<GetAllExamDto>>(examList); // TYPO

        // ORTA DÜZELTME: Null ve empty kontrolü eklendi
        if (examtListMapping == null || !examtListMapping.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllExamDto>>(null, ConstantsMessages.ExamListFailedMessage);
        }
        var firstExam = examtListMapping.ToList()[0]; // Artık güvenli

        return new SuccessDataResult<IEnumerable<GetAllExamDto>>(examtListMapping, ConstantsMessages.ExamListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdExamDto>> GetByIdAsync(string id, bool track = true)
    {
        var hasExam = await _unitOfWork.Exams.GetByIdAsync(id, false);
        var examResultMapping = _mapper.Map<GetByIdExamDto>(hasExam);
        return new SuccessDataResult<GetByIdExamDto>(examResultMapping, ConstantsMessages.ExamGetByIdSuccessMessage);
    }
    public async Task<IResult> CreateAsync(CreateExamDto entity)
    {
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var addedExamMapping = _mapper.Map<Exam>(entity);

        // ORTA DÜZELTME: Null kontrolü eklendi
        if (addedExamMapping == null)
        {
            return new ErrorResult("Mapping failed");
        }
        var examName = addedExamMapping.Name; // Artık güvenli

        // ZOR: Async/await anti-pattern - async metot içinde .Wait() kullanımı deadlock'a sebep olabilir
        _unitOfWork.Exams.CreateAsync(addedExamMapping).Wait(); // ZOR: Anti-pattern - await kullanılmalıydı
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamCreateSuccessMessage);
        }
        // KOLAY: Noktalı virgül eksikliği
        return new ErrorResult(ConstantsMessages.ExamCreateFailedMessage); // TYPO: ; eksik
    }

    public async Task<IResult> Remove(DeleteExamDto entity)
    {
        var deletedExamMapping = _mapper.Map<Exam>(entity); // ORTA SEVİYE: ID kontrolü eksik - entity ID'si null/empty olabilir
        _unitOfWork.Exams.Remove(deletedExamMapping);
        var result = await _unitOfWork.CommitAsync(); // ZOR SEVİYE: Transaction yok - başka işlemler varsa rollback olmaz
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdateExamDto entity)
    {
        var updatedExamMapping = _mapper.Map<Exam>(entity);
        _unitOfWork.Exams.Update(updatedExamMapping);
        var result = await _unitOfWork.CommitAsync();
        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.ExamUpdateSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.ExamUpdateFailedMessage);
    }
}
