using APBD_cw3.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_cw3.DAL
{
    public class Service : IDbService
    {
        private readonly string _connection = "Data Source=db-mssql;Initial Catalog=s18621;Integrated Security=True";
        public IEnumerable<Student> GetStudents()
        { 
                List<Student> _students = new List<Student>();
                using (var client = new SqlConnection(_connection))
                using (var com = new SqlCommand())
                {
                    com.Connection = client;
                    com.CommandText = "SELECT * FROM Student";

                    client.Open();
                    var dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        var st = new Student();
                        st.FirstName = dr["FirstName"].ToString();
                        st.LastName = dr["LastName"].ToString();
                        st.BirthDate = dr["BirthDate"].ToString();
                        st.IndexNumber = dr["IndexNumber"].ToString();

                        _students.Add(st);
                    }
                }        
            return _students;
        }
        public IEnumerable<Enrollment> GetEnrollments(string number)
        {
            List<Enrollment> _enrollments = new List<Enrollment>();
            using (var client = new SqlConnection(_connection))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = @"SELECT 
                        e.IdEnrollment, e.Semester, e.StartDate, e.StartDate, st.IdStudy, st.Name FROM Student s
                        INNER JOIN Enrollment e on e.IdEnrollment = s.IdEnrollment
                        INNER JOIN Studies st on st.IdStudy = e.IdEnrollment
                        WHERE 
                        s.IndexNumber = @index";
                com.Parameters.AddWithValue("index", number);

                client.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Enrollment();
                    st.IdEnrollment = int.Parse(dr["IdEnrollment"].ToString());
                    st.Semester = int.Parse(dr["Semester"].ToString());
                    st.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                    st.IdStudy = new Studies
                    {
                        IdStudy = int.Parse(dr["IdStudy"].ToString()),
                        Name = dr["Name"].ToString(),
                    };                
                    _enrollments.Add(st);
                };
            }
                return _enrollments;
        }

    }
}