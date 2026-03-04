

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]

public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_studentService.GetStudents());
    }

}