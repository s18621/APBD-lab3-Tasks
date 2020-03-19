using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD_cw3.DAL;
using APBD_cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace APBD_cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        public String GetStudent()
        {
            return "Kowalski, Malewski, Andrzejewski";
        }

        [HttpGet("{id}")]
            public IActionResult GetStudent(int id)
        {
            if(id==1)
            {
                return Ok("Kowalski");
            }else if (id == 2)
            {
                return Ok("Malewski");
            }
            return NotFound("Nie znaleziono studenta");
        }

        [HttpGet]
            public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }
        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            //... add to database
            //... generating index number
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }
        [HttpDelete]
        public IActionResult DeleteStudents(int id)
        {
            //... todo
            if (id == 1 || id == 2 || id == 3)
                return Ok("Usuwanie ukonczone");
            else return NotFound("Nie ma studenta o tym id");
        }

        [HttpPut]
        public IActionResult PutStudents(int id)
        {
            //...todo
            if (id == 1 || id == 2 || id == 3)
                return Ok("Aktualizacja dokonczona");
            else return NotFound("Nie ma studenta o tym id");
        }
       

    }
}