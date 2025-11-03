using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.EntityLayer.Dto.StudentDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;

namespace CourseApp.ServiceLayer.Concrete;

public class StudentManager : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public StudentManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllStudentDto>>> GetAllAsync(bool track = true)
    {
        var studentList = await _unitOfWork.Students.GetAll(track).ToListAsync();
        var studentListMapping = _mapper.Map<IEnumerable<GetAllStudentDto>>(studentList);
        if (!studentList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllStudentDto>>(null, ConstantsMessages.StudentListFailedMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllStudentDto>>(studentListMapping, ConstantsMessages.StudentListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdStudentDto>> GetByIdAsync(string id, bool track = true)
    {
        // ORTA: Null check eksik - id null/empty olabilir
        // ORTA: Null reference exception - hasStudent null olabilir ama kontrol edilmiyor
        var hasStudent = await _unitOfWork.Students.GetByIdAsync(id, false);
        var hasStudentMapping = _mapper.Map<GetByIdStudentDto>(hasStudent);
        // ORTA: Null reference - hasStudentMapping null olabilir ama kullanılıyor
        var name = hasStudentMapping.Name; // Null reference riski
        return new SuccessDataResult<GetByIdStudentDto>(hasStudentMapping, ConstantsMessages.StudentGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateStudentDto entity)
    {
        if(entity == null) return new ErrorResult("Null");
        
}
