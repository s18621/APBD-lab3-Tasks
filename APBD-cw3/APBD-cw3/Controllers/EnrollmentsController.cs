using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APBD_cw3.DAL;
using APBD_cw3.DTOs.Requests;
using APBD_cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD_cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        private readonly IDbService _dbService;
        public EnrollmentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        public IActionResult PostStudent(EnrollStudentRequest request)
        {
            var study = _dbService.GetStudies(request.Studies);
            if (study == null) return BadRequest();
            else
            {
                var newStudent = new Student();
                newStudent.IndexNumber = request.IndexNumber;
                newStudent.FirstName = request.FirstName;
                newStudent.LastName = request.LastName;
                newStudent.BirthDate = DateTime.Parse(request.Birthdate.ToString());
                _dbService.Register(newStudent, study.IdStudy);
                return StatusCode(201);
            }
        }
        [HttpPost("Promotions")]
        public IActionResult PromoteStudent(EnrollRequest request)
        {      
                var enrollment =  _dbService.GetEnrollment(request.Studies, request.Semester);
                if (enrollment == null) return NotFound();
                
                else
                {
                    _dbService.Promote(request.Studies, request.Semester);
                    return StatusCode(201);
                }
            
        }


    }
}