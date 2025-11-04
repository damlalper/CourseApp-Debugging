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
        // ORTA DÜZELTME: Null ve empty kontrolü eklendi
        if (string.IsNullOrEmpty(id))
        {
            return new ErrorDataResult<GetByIdStudentDto>(null, "Invalid ID");
        }

        var hasStudent = await _unitOfWork.Students.GetByIdAsync(id, false);
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (hasStudent == null)
        {
            return new ErrorDataResult<GetByIdStudentDto>(null, ConstantsMessages.StudentGetByIdFailedMessage);
        }

        var hasStudentMapping = _mapper.Map<GetByIdStudentDto>(hasStudent);
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (hasStudentMapping == null)
        {
            return new ErrorDataResult<GetByIdStudentDto>(null, "Mapping failed");
        }
        var name = hasStudentMapping.Name; // Artık güvenli
        return new SuccessDataResult<GetByIdStudentDto>(hasStudentMapping, ConstantsMessages.StudentGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateStudentDto entity)
    {
        if(entity == null) return new ErrorResult("Null");
        throw new NotImplementedException();
    }

    public Task<IResult> Update(UpdateStudentDto entity)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> Remove(DeleteStudentDto entity)
    {
        throw new NotImplementedException();
    }
}
