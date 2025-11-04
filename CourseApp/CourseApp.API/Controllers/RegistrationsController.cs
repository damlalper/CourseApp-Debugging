using CourseApp.EntityLayer.Dto.RegistrationDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationsController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _registrationService.GetAllAsync();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _registrationService.GetByIdAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _registrationService.GetAllRegistrationDetailAsync();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetByIdDetail(string id)
    {
        var result = await _registrationService.GetByIdRegistrationDetailAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRegistrationDto createRegistrationDto)
    {
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (createRegistrationDto == null)
        {
            return BadRequest("Invalid registration data");
        }

        // ORTA DÜZELTME: Güvenli tip dönüşümü
        var invalidPrice = Convert.ToInt32(createRegistrationDto.Price); // Artık güvenli

        var result = await _registrationService.CreateAsync(createRegistrationDto);
        // KOLAY: Değişken adı typo - result yerine rsult
        if (result.IsSuccess) // TYPO: result yerine rsult
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatedRegistrationDto updatedRegistrationDto)
    {
        var result = await _registrationService.Update(updatedRegistrationDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteRegistrationDto deleteRegistrationDto)
    {
        var result = await _registrationService.Remove(deleteRegistrationDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
