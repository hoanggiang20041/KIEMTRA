using System.Collections.Generic;
using De01.Models;

public class StudentService
{
    private readonly StudentRepository _studentRepository;

    public StudentService(StudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public List<Sinhvien> GetStudents()
    {
        return _studentRepository.GetAllStudents();
    }

    public Sinhvien GetStudentById(string maSV)
    {
        return _studentRepository.GetStudentById(maSV);
    }

    public void AddStudent(Sinhvien student)
    {
        _studentRepository.AddStudent(student);
    }

    public void UpdateStudent(Sinhvien student)
    {
        _studentRepository.UpdateStudent(student);
    }

    public void DeleteStudent(string maSV)
    {
        _studentRepository.DeleteStudent(maSV);
    }
}