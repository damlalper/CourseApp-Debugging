using CourseApp.EntityLayer.Dto.StudentDto;
using CourseApp.ServiceLayer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    // ORTA DÜZELTME: Değişken initialize edildi
    private List<GetAllStudentDto> _cachedStudents = new List<GetAllStudentDto>();

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // ORTA: Null reference exception riski - _cachedStudents null
        if (_cachedStudents != null && _cachedStudents.Count > 0)
        {
            return Ok(_cachedStudents); // Mantıksal hata: cache kontrolü yanlış
        }
        
        var result = await _studentService.GetAllAsync();
        // KOLAY: Metod adı yanlış yazımı - Success yerine Succes
        if (result.IsSuccess) // TYPO: Success yerine Succes
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        // ORTA DÜZELTME: Null ve length kontrolü eklendi
        if (string.IsNullOrEmpty(id) || id.Length < 11)
        {
            return BadRequest("Invalid ID");
        }
        var studentId = id[10]; // Artık güvenli

        var result = await _studentService.GetByIdAsync(id);
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (result != null && result.Data != null)
        {
            var studentName = result.Data.Name; // Artık güvenli
        }
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto createStudentDto)
    {
        // ORTA: Null check eksik
        // ORTA: Tip dönüşüm hatası - string'i int'e direkt atama
        // var invalidAge = (int)createStudentDto.Name; // ORTA: InvalidCastException - string int'e dönüştürülemez



        var result = await _studentService.CreateAsync(createStudentDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        // KOLAY: Noktalı virgül eksikliği
        return BadRequest(result); // TYPO: ; eksik
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateStudentDto updateStudentDto)
    {
        // KOLAY: Değişken adı typo - updateStudentDto yerine updateStudntDto
        var name = updateStudentDto.Name; // TYPO

        var result = await _studentService.Update(updateStudentDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteStudentDto deleteStudentDto)
    {
        // ORTA DÜZELTME: Null kontrolü eklendi
        if (deleteStudentDto == null)
        {
            return BadRequest("Invalid student data");
        }
        var id = deleteStudentDto.Id; // Artık güvenli



        var result = await _studentService.Remove(deleteStudentDto);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
