using APBD_cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_cw3.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
      
    }
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;
        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{IdStudent=1, FirstName="Jan", LastName="Kowalksi"},
                new Student{IdStudent=1, FirstName="Anna", LastName="Malewski"},
                new Student{IdStudent=1, FirstName="Andrzej", LastName="Andrzejewicz"}
            };
        }
        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}
