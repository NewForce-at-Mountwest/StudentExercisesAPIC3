using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercises.Models;

namespace StudentExercises.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CohortsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT Id, Name FROM Cohort";
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            List<Cohort> cohorts = new List<Cohort>();

        //            while (reader.Read())
        //            {

        //                    Cohort cohort = new Cohort
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                        Name = reader.GetString(reader.GetOrdinal("Name"))
        //                    };

        //                    cohorts.Add(cohort);

        //            }
        //            reader.Close();

        //            return Ok(cohorts);
        //        }
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {


        //            cmd.CommandText = @"SELECT c.Id, c.Name, s.Id AS 'Student Id', s.FirstName, s.LastName, s.SlackHandle, s.CohortId, I.Id, i.FirstName, i.LastName,       i.SlackHandle, i.Specialty, i.CohortId FROM  Cohort c
        //                LEFT JOIN Student s on c.Id = s.CohortId
        //                LEFT JOIN Instructor i on c.Id = i.CohortId";
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            List<Cohort> cohorts = new List<Cohort>();

        //            while (reader.Read())
        //            {
        //                bool cohortExistInList = cohorts.Any(c => c.Id == reader.GetInt32(reader.GetOrdinal("Id")));
        //                if (!cohortExistInList)
        //                {
        //                    Cohort cohort = new Cohort
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                        Name = reader.GetString(reader.GetOrdinal("Name"))
        //                    };

        //                    bool studentExistInList = cohort.StudentList.Any(s => s.Id == reader.GetInt32(reader.GetOrdinal("Student Id")));
        //                    bool nullStudent = reader.IsDBNull(reader.GetOrdinal("Student Id"));
        //                    if (!nullStudent && !studentExistInList)
        //                    {
        //                        Student student = new Student
        //                        {
        //                            Id = reader.GetInt32(reader.GetOrdinal("Student Id")),
        //                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                           SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
        //                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

        //                        };
        //                        cohort.StudentList.Add(student);
        //                    }

        //                    cohorts.Add(cohort);
        //                } else
        //                {
        //                    Cohort cohortInList = cohorts.FirstOrDefault(c => c.Id == reader.GetInt32(reader.GetOrdinal("Id")));
        //                    bool studentExistInList = cohortInList.StudentList.Any(s => s.Id == reader.GetInt32(reader.GetOrdinal("Student Id")));
        //                    bool nullStudent = reader.IsDBNull(reader.GetOrdinal("Student Id"));


        //                    if (!nullStudent && !studentExistInList)
        //                    {
        //                        Student student = new Student
        //                        {
        //                            Id = reader.GetInt32(reader.GetOrdinal("Student Id")),
        //                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
        //                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

        //                        };
        //                        cohortInList.StudentList.Add(student);
        //                    }

        //                }







        //            }
        //            reader.Close();

        //            return Ok(cohorts);
        //        }
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {


                    cmd.CommandText = @"SELECT c.Id, c.Name, i.Id AS 'Instructor Id', i.FirstName AS 'Instructor FirstName', i.LastName AS 'Instructor LastName', i.SlackHandle AS 'Instructor SlackHandle',      i.Specialty, i.CohortId AS 'Instructor CohortId', s.Id AS 'Student Id', s.FirstName, s.LastName, s.SlackHandle, i.CohortId FROM Cohort c
                      LEFT JOIN Instructor i on c.Id = i.CohortId
                      LEFT JOIN Student s on c.Id = s.CohortId";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {

                        if (!cohorts.Any(c => c.Id == reader.GetInt32(reader.GetOrdinal("Id"))))
                        {
                            Cohort cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("Student Id")))
                            {
                                Student student = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Student Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

                                };
                                cohort.StudentList.Add(student);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("Instructor Id")))
                            {
                                Instructor instructor = new Instructor
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Instructor Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("Instructor FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("Instructor LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("Instructor SlackHandle")),
                                    Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("Instructor CohortId")),

                                };

                                cohort.InstructorList.Add(instructor);
                            }

                            cohorts.Add(cohort);
                        }
                        else
                        {
                            Cohort cohortAlreadyInTheList = cohorts.FirstOrDefault(c => c.Id == reader.GetInt32(reader.GetOrdinal("CohortId")));
                            bool studentExists = !reader.IsDBNull(reader.GetOrdinal("Student Id"));
                            bool studentIsNotAlreadyInTheList = !cohortAlreadyInTheList.StudentList.Any(s => s.Id == reader.GetInt32(reader.GetOrdinal("Student Id")));
                            if (studentExists && studentIsNotAlreadyInTheList)
                            {
                                Student student = new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Student Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

                                };
                                cohortAlreadyInTheList.StudentList.Add(student);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("Instructor Id")) && !cohorts.FirstOrDefault(c => c.Id == reader.GetInt32(reader.GetOrdinal("Instructor CohortId"))).InstructorList.Any(s => s.Id == reader.GetInt32(reader.GetOrdinal("Instructor Id"))))
                            {
                                Instructor instructor = new Instructor
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Instructor Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("Instructor FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("Instructor LastName")),
                                    SlackHandle = reader.GetString(reader.GetOrdinal("Instructor SlackHandle")),
                                    Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
                                    CohortId = reader.GetInt32(reader.GetOrdinal("Instructor CohortId")),

                                };
                                cohorts.FirstOrDefault(c => c.Id == instructor.CohortId).InstructorList.Add(instructor);
                            }

                        }
                    }
                    reader.Close();

                    return Ok(cohorts);
                }
            }
        }

        [HttpGet("{id}", Name = "GetCohort")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                     SELECT c.Id, c.Name, i.Id AS 'Instructor Id', i.FirstName AS 'Instructor FirstName', i.LastName AS 'Instructor LastName', i.SlackHandle AS 'Instructor SlackHandle',      i.Specialty, i.CohortId AS 'Instructor CohortId', s.Id AS 'Student Id', s.FirstName, s.LastName, s.SlackHandle, i.CohortId FROM Cohort c
                      LEFT JOIN Instructor i on c.Id = i.CohortId
                      LEFT JOIN Student s on c.Id = s.CohortId
                                     WHERE c.ID = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;

                    if (reader.Read())
                    {
                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }
                    reader.Close();

                    return Ok(cohort);
                }
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] Cohort cohort)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"INSERT INTO Cohort (FirstName, LastName, SlackHandle, Specialty, CohortId FROM Cohort)
        //                                OUTPUT INSERTED.Id
        //                                VALUES (@firstName, @lastName, @slackHandle, @specialty,  @cohortId)";
        //            cmd.Parameters.Add(new SqlParameter("@firstName", cohort.FirstName));
        //            cmd.Parameters.Add(new SqlParameter("@lastName", cohort.LastName));
        //            cmd.Parameters.Add(new SqlParameter("@slackHandle", cohort.SlackHandle));
        //            cmd.Parameters.Add(new SqlParameter("@specialty", cohort.Specialty));
        //            cmd.Parameters.Add(new SqlParameter("@cohortId", cohort.CohortId));

        //            int newId = (int)cmd.ExecuteScalar();
        //            cohort.Id = newId;
        //            return CreatedAtRoute("GetCohort", new { id = newId }, cohort);
        //        }
        //    }
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Cohort cohort)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"UPDATE Cohort
        //                                    SET FirstName = @firstName,
        //                                        LastName = @lastName,
        //                                        SlackHandle = @slackHandle,
        //                                        Specialty = @specialty,
        //                                        CohortId = @cohortId
        //                                    WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@firstName", cohort.FirstName));
        //                cmd.Parameters.Add(new SqlParameter("@lastName", cohort.LastName));
        //                cmd.Parameters.Add(new SqlParameter("@slackHandle", cohort.SlackHandle));
        //                cmd.Parameters.Add(new SqlParameter("@specialty", cohort.Specialty));
        //                cmd.Parameters.Add(new SqlParameter("@cohortId", cohort.CohortId));
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = cmd.ExecuteNonQuery();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!CohortExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"DELETE FROM Cohort WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = cmd.ExecuteNonQuery();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!CohortExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //private bool CohortExists(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                SELECT Id, Name, Language
        //                FROM Cohort
        //                WHERE Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));

        //            SqlDataReader reader = cmd.ExecuteReader();
        //            return reader.Read();
        //        }
        //    }
        //}
    }
}
