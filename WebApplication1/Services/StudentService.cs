public class StudentService : IStudentService
{
    public List<string> GetStudents()
    {
        return new List<string>
        {
            "ali",
            "sara",
            "aman"
        };
    }
}