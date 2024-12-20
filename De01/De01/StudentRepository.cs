using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using De01.Models;

public class StudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Sinhvien> GetAllStudents()
    {
        return _context.Sinhviens.ToList();
    }

    public Sinhvien GetStudentById(string maSV)
    {
        return _context.Sinhviens.SingleOrDefault(sv => sv.MaSV == maSV);
    }

    public void AddStudent(Sinhvien student)
    {
        _context.Sinhviens.Add(student);
        _context.SaveChanges();
    }

    public void UpdateStudent(Sinhvien student)
    {
        _context.Entry(student).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void DeleteStudent(string maSV)
    {
        var student = GetStudentById(maSV);
        if (student != null)
        {
            _context.Sinhviens.Remove(student);
            _context.SaveChanges();
        }
    }
}