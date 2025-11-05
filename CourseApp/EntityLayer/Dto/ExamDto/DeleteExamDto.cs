namespace CourseApp.EntityLayer.Dto.ExamDto;

public class DeleteExamDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public DateTime Date { get; set; }
}
