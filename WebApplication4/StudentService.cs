using System;

public class StudentService:IStudentService
{

    private readonly List<string> _students;

    public StudentService()
    {
    _students=new List<string> { "ali","sara","john"}

    }
}