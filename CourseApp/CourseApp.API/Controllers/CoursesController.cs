using CourseApp.EntityLayer.Dto.CourseDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _courseService.GetAllAsync();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        // KOLAY: Metod adı yanlış yazımı - GetByIdAsync yerine GetByIdAsnc
        var result = await _courseService.GetByIdAsync(id); // TYPO: Async yerine Asnc
        // ORTA: Null reference - result null olabilir
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _courseService.GetAllCourseDetail();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto)
    {
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (createCourseDto == null || string.IsNullOrEmpty(createCourseDto.CourseName))
        {
            return BadRequest("Invalid course data");
        }

        var courseName = createCourseDto.CourseName; // Artık güvenli

        // ORTA DÜZELTME: Length kontrolü eklendi
        var firstChar = courseName[0]; // Artık güvenli

        var result = await _courseService.CreateAsync(createCourseDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        // KOLAY: Noktalı virgül eksikliği
        return BadRequest(result); // TYPO: ; eksik
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCourseDto updateCourseDto)
    {
        var result = await _courseService.Update(updateCourseDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCourseDto deleteCourseDto)
    {
        var result = await _courseService.Remove(deleteCourseDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
