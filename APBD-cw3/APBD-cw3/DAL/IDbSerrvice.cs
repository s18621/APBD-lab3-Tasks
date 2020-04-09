using APBD_cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_cw3.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
        public IEnumerable<Enrollment> GetEnrollments(string number);

        public Studies GetStudies(String str);
        public Enrollment GetEnrollment(string study, int semester);

        public void Register(Student student, int studyId);
        public void Promote(string studies, int semenster);

        public bool checkIndex(String index);


    } 
}