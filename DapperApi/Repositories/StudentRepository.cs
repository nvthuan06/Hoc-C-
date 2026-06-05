using System;
using Dapper;
using DapperApi.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DapperApi.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly string? _connStr;

    public StudentRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("DefaultConnection");
    }

    public IDbConnection Connection() => new SqlConnection(_connStr);
    public IEnumerable<Student> GetAll()
    {
        using var db = Connection();
        return db.Query<Student>("SELECT * FROM Students");
    }
    public Student? GetById(int id)
    {
        using var db = Connection();
        return db.QueryFirstOrDefault<Student>("SELECT * FROM Students WHERE Id = @Id", new { Id = id });
    }
    public void Create(Student student)
    {
        using var db = Connection();
        db.Execute("INSERT INTO Students (Name, Age, Email) VALUES (@Name, @Age, @Email)", student);
    }
    public void Update(Student student)
    {
        using var db = Connection();
        db.Execute("UPDATE Students SET Name = @Name, Age = @Age, Email = @Email WHERE Id = @Id", student);
    }
    public void Delete(int id)
    {
        using var db = Connection();
        db.Execute("DELETE FROM Students WHERE Id = @Id", new { Id = id });
    }

    public IEnumerable<Student> GetByName(string name)
    {
        using var db = Connection();
        return db.Query<Student>("SELECT * FROM Students WHERE Name LIKE @Name", new { Name = $"%{name}%" });
    }
    public IEnumerable <StudentWithCourses > GetAllWithCourses()
    {
        var sql = @"
        SELECT s.Id, s.Name , c.Id, c.CourseName
        FROM Students s
        JOIN StudentCourses sc ON s.Id = sc.StudentId
        JOIN Courses c ON sc.CourseId = c.Id
        ORDER BY s.Id";

        using var db = Connection();
        var dict = new Dictionary <int, StudentWithCourses >();
        return db.Query <StudentWithCourses , Course , StudentWithCourses >(
            sql,
            (student , course) =>
            {
                if (!dict.TryGetValue(student.Id, out var existing))
                {
                    existing = student;
                    dict[student.Id] = existing;
                }
                existing.Courses.Add(course);
                return existing;
            },
            splitOn: "Id" // cot phan tach Student / Course
        );
        
        return dict.Values;
    }
}