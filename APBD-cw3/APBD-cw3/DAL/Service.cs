using APBD_cw3.DTOs.Requests;
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

        public object DbNull { get; private set; }

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
                    st.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
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
                    st.Study = new Studies
                    {
                        IdStudy = int.Parse(dr["IdStudy"].ToString()),
                        Name = dr["Name"].ToString(),
                    };
                    _enrollments.Add(st);
                };
            }
            return _enrollments;
        }
        public Studies GetStudies(String studies)
        {
            using (var conection = new SqlConnection(_connection))
            using (var command = new SqlCommand())
            {
                command.Connection = conection;
                command.CommandText = @"SELECT idstudy, Name FROM Studies WHERE Name = @name";
                command.Parameters.AddWithValue("name", studies);

                conection.Open();
                try
                {
                    var dr = command.ExecuteReader();
                    dr.Read();
                    var _study = new Studies
                    {
                        IdStudy = int.Parse(dr["idstudy"].ToString()),
                        Name = dr["name"].ToString()
                    };
                    return _study;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }

        public void Register(Student student, int studyId)
        {
            using(var conection = new SqlConnection(_connection))
            using (var command = new SqlCommand())
            { 
                conection.Open();
                var transact = conection.BeginTransaction();

                command.Connection = conection;
                command.Transaction = transact;
                command.CommandText = @"DECLARE @_enrollment int
                                        SELECT @_enrollment = e.idenrollment FROM enrollment e INNER JOIN studies s ON e.idstudy = s.idstudy WHERE e.idstudy = @_study AND e.semester = 1;
                                        IF @_enrollment IS NULL
                                        BEGIN 
                                        SELECT @_enrollment = max(idenrollment) + 1 
                                        FROM enrollment;
                                        INSERT INTO enrollment 
                                        VALUES (@_enrollment, 1, @_study, getdate());
                                        END
                                        INSERT INTO Student values (@_index, @_firstname, @_lastname, @_birthdate, @_enrollment)";
                command.Parameters.AddWithValue("_study", studyId);
                command.Parameters.AddWithValue("_index", student.IndexNumber);
                command.Parameters.AddWithValue("_firstName", student.FirstName);
                command.Parameters.AddWithValue("_lastName", student.LastName);
                command.Parameters.AddWithValue("_birthDate", student.BirthDate);
                command.ExecuteNonQuery();
                try
                {
                    transact.Commit();
                }catch(Exception exc)
                {
                    transact.Rollback();
                }
            }
        }
        public Enrollment GetEnrollment(string study, int semester)
        {
            using (var conection = new SqlConnection(_connection))
            using (var command = new SqlCommand())
                {
                    command.Connection = conection;
                    command.CommandText = @"SELECT e.idenrollment, e.semester, e.startdate, s.idstudy, s.name
                                    FROM Enrollment e
                                    INNER JOIN studies s ON s.idstudy = e.idstudy
                                    WHERE s.name = @studyname
                                    AND e.semester = @semester";
                    command.Parameters.AddWithValue("studyname", study);
                    command.Parameters.AddWithValue("semester", semester);
                    conection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        return new Enrollment
                        {
                            IdEnrollment = int.Parse(reader["idenrollment"].ToString()),
                            Semester = int.Parse(reader["semester"].ToString()),
                            StartDate = DateTime.Parse(reader["startdate"].ToString()),
                            Study = new Studies
                            {
                                IdStudy = int.Parse(reader["idstudy"].ToString()),
                                Name = reader["name"].ToString(),
                            }
                        };
                    }
            }
        }
        public void Promote(string studies, int semester)
        {
            using (var conection = new SqlConnection(_connection))
            using (var command = new SqlCommand())
            {
                command.Connection = conection;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "PromoteStudents";
                command.Parameters.AddWithValue("studies", studies);
                command.Parameters.AddWithValue("semester", semester);
                conection.Open();
                command.ExecuteNonQueryAsync();
            }
             /*Create PROCEDURE PromoteStudents @studies varchar(255), @semester int
                AS
                DECLARE @tmp int
                BEGIN
                    IF EXISTS(SELECT 1 FROM Studies WHERE name = @studies)
                    BEGIN
                        INSERT INTO Studies VALUES((SELECT max(idstudy) + 1 FROM Studies),@studies);
                                    RETURN
                    END;
                DECLARE cursor CURSOR FOR SELECT IdEnrollment FROM enrollment e
                INNER JOIN studies s on e.idstudy = s.idstudy
                WHERE s.name = @studies and e.semester = @semester;
                OPEN cursor
                FETCH FROM cursor INTO @tmp
                UPDATE Enrollment
                SET Semester = Semester + 1
                WHERE IdEnrollment = @tmp
                FETCH FROM cur INTO @tmp
                CLOSE cur;
                END;
                */
        }
        public bool checkIndex(string index)
        {
            using (var connection = new SqlConnection(_connection))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT 1 FROM student WHERE indexNumber = @index";
                command.Parameters.AddWithValue("index", index);
                connection.Open();
                var dr = command.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    dr.Close();
                    return true;
                }
                else
                {
                    dr.Close();
                    return false;
                }

            }
        }
    }
}