using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DapperApi.Repositories;
using DapperApi.Models;

namespace DapperApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _repo;
        public StudentController(IStudentRepository repo)
        {
            _repo = repo;
        }
        [HttpGet]
        public IActionResult GetAll() => Ok(_repo.GetAll());
        [HttpGet("{id}")]
        public IActionResult GetById(int id)        {
            var student = _repo.GetById(id);
            if (student == null) return NotFound();
            return Ok(student);
        }
        [HttpGet("search")]
        public IActionResult GetByName([FromQuery] string name)
        {
            var students = _repo.GetByName(name);
            return Ok(students);
        }

        [HttpPost]
        public IActionResult Create(Student student)        
        {
            _repo.Create(student);
            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }
        [HttpPut("{id}")]
        public IActionResult Update([FromBody] Student student)
        {
            _repo.Update(student);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repo.Delete(id);
            return NoContent();
        }
        [HttpGet("courses")]
        public IActionResult GetAllWithCourses()
        {
            var studentsWithCourses = _repo.GetAllWithCourses();
            return Ok(studentsWithCourses);
        }
    }
}
