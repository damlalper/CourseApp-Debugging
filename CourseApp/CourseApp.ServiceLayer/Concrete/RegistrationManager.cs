using AutoMapper;
using CourseApp.DataAccessLayer.UnitOfWork;
using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.EntityLayer.Entity;
using CourseApp.ServiceLayer.Abstract;
using CourseApp.ServiceLayer.Utilities.Constants;
using CourseApp.ServiceLayer.Utilities.Result;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseApp.ServiceLayer.Concrete;

public class RegistrationManager : IRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public RegistrationManager(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IDataResult<IEnumerable<GetAllRegistrationDto>>> GetAllAsync(bool track = true)
    {
        var registrationList = await _unitOfWork.Registrations.GetAll(track).ToListAsync();
        var registrationListMapping = _mapper.Map<IEnumerable<GetAllRegistrationDto>>(registrationList);
        if (!registrationList.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllRegistrationDto>>(null, ConstantsMessages.RegistrationListFailedMessage);
        }
        return new SuccessDataResult<IEnumerable<GetAllRegistrationDto>>(registrationListMapping, ConstantsMessages.RegistrationListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdRegistrationDto>> GetByIdAsync(string id, bool track = true)
    {
        var hasRegistration = await _unitOfWork.Registrations.GetByIdAsync(id, track);
        var hasRegistrationMapping = _mapper.Map<GetByIdRegistrationDto>(hasRegistration);
        return new SuccessDataResult<GetByIdRegistrationDto>(hasRegistrationMapping, ConstantsMessages.RegistrationGetByIdSuccessMessage);
    }

    public async Task<IResult> CreateAsync(CreateRegistrationDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var createdRegistration = _mapper.Map<Registration>(entity);
        if (createdRegistration == null)
        {
            return new ErrorResult("Mapping failed");
        }

        await _unitOfWork.Registrations.CreateAsync(createdRegistration);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.RegistrationCreateSuccessMessage);
        }

        return new ErrorResult(ConstantsMessages.RegistrationCreateFailedMessage);
    }

    public async Task<IResult> Remove(DeleteRegistrationDto entity)
    {
        if (entity == null || string.IsNullOrEmpty(entity.Id))
        {
            return new ErrorResult("Registration data cannot be null.");
        }

        var registrationToDelete = await _unitOfWork.Registrations.GetByIdAsync(entity.Id);
        if (registrationToDelete == null)
        {
            return new ErrorResult(ConstantsMessages.RegistrationGetByIdFailedMessage);
        }

        _unitOfWork.Registrations.Remove(registrationToDelete);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.RegistrationDeleteSuccessMessage);
        }
        return new ErrorResult(ConstantsMessages.RegistrationDeleteFailedMessage);
    }

    public async Task<IResult> Update(UpdatedRegistrationDto entity)
    {
        if (entity == null)
        {
            return new ErrorResult("Entity cannot be null");
        }

        var existingRegistration = await _unitOfWork.Registrations.GetByIdAsync(entity.Id, true);
        if (existingRegistration == null)
        {
            return new ErrorResult(ConstantsMessages.RegistrationGetByIdFailedMessage);
        }

        _mapper.Map(entity, existingRegistration);

        _unitOfWork.Registrations.Update(existingRegistration);
        var result = await _unitOfWork.CommitAsync();

        if (result > 0)
        {
            return new SuccessResult(ConstantsMessages.RegistrationUpdateSuccessMessage);
        }
        
        return new ErrorResult(ConstantsMessages.RegistrationUpdateFailedMessage);
    }

    public async Task<IDataResult<IEnumerable<GetAllRegistrationDetailDto>>> GetAllRegistrationDetailAsync(bool track = true)
    {
        var registrationData = await _unitOfWork.Registrations.GetAllRegistrationDetail(track).ToListAsync();
        var registrationDataMapping = _mapper.Map<IEnumerable<GetAllRegistrationDetailDto>>(registrationData);

        if (registrationDataMapping == null || !registrationDataMapping.Any())
        {
            return new ErrorDataResult<IEnumerable<GetAllRegistrationDetailDto>>(null, ConstantsMessages.RegistrationListFailedMessage);
        }

        return new SuccessDataResult<IEnumerable<GetAllRegistrationDetailDto>>(registrationDataMapping, ConstantsMessages.RegistrationListSuccessMessage);
    }

    public async Task<IDataResult<GetByIdRegistrationDetailDto>> GetByIdRegistrationDetailAsync(string id, bool track = true)
    {
        var registration = await _unitOfWork.Registrations.GetByIdRegistrationDetail(id, track);

        if (registration == null)
        {
            return new ErrorDataResult<GetByIdRegistrationDetailDto>(null, ConstantsMessages.RegistrationGetByIdFailedMessage);
        }

        var registrationMapping = _mapper.Map<GetByIdRegistrationDetailDto>(registration);
        return new SuccessDataResult<GetByIdRegistrationDetailDto>(registrationMapping, ConstantsMessages.RegistrationGetByIdSuccessMessage);
    }
}
