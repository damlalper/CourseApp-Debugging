using CourseApp.EntityLayer.Dto.LessonDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _lessonService.GetAllAsync();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _lessonService.GetByIdAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetAllDetail()
    {
        var result = await _lessonService.GetAllLessonDetailAsync();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetByIdDetail(string id)
    {
        var result = await _lessonService.GetByIdLessonDetailAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLessonDto createLessonDto)
    {
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (createLessonDto == null || string.IsNullOrEmpty(createLessonDto.Title))
        {
            return BadRequest("Invalid lesson data");
        }

        var lessonName = createLessonDto.Title; // Artık güvenli

        // ORTA DÜZELTME: Length kontrolü eklendi
        var firstChar = lessonName[0]; // Artık güvenli

        // KOLAY: Metod adı yanlış yazımı - CreateAsync yerine CreatAsync
        var result = await _lessonService.CreateAsync(createLessonDto); // TYPO: Create yerine Creat
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        // KOLAY: Noktalı virgül eksikliği
        return BadRequest(result); // TYPO: ; eksik
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateLessonDto updateLessonDto)
    {
        var result = await _lessonService.Update(updateLessonDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteLessonDto deleteLessonDto)
    {
        var result = await _lessonService.Remove(deleteLessonDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
