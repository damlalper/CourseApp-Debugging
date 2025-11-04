using CourseApp.EntityLayer.Dto.InstructorDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InstructorsController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorsController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _instructorService.GetAllAsync();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _instructorService.GetByIdAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatedInstructorDto createdInstructorDto)
    {
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (createdInstructorDto == null || string.IsNullOrEmpty(createdInstructorDto.Name))
        {
            return BadRequest("Invalid instructor data");
        }

        var instructorName = createdInstructorDto.Name; // Artık güvenli

        // ORTA DÜZELTME: Length kontrolü eklendi
        var firstChar = instructorName[0]; // Artık güvenli

        // ORTA: Tip dönüşüm hatası - string'i int'e direkt cast
        // var invalidAge = (int)instructorName; // ORTA: InvalidCastException

        var result = await _instructorService.CreateAsync(createdInstructorDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatedInstructorDto updatedInstructorDto)
    {
        var result = await _instructorService.Update(updatedInstructorDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeletedInstructorDto deletedInstructorDto)
    {
        var result = await _instructorService.Remove(deletedInstructorDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
