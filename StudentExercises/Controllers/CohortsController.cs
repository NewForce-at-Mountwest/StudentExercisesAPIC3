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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {


                    cmd.CommandText = @"SELECT Cohort.Name, Cohort.Id,
Student.Id AS 'Student Id', Student.FirstName AS 'Student First Name', Student.LastName AS 'Student LastName', Student.SlackHandle AS 'Student SlackHandle', Student.CohortId AS 'Student CohortId',
Instructor.Id AS 'Instructor Id', Instructor.FirstName AS 'Instructor FirstName', Instructor.LastName AS 'Instructor LastName', Instructor.SlackHandle AS 'Instructor SlackHandle', Instructor.CohortId AS 'Instructor CohortId'
FROM Cohort 
LEFT JOIN Student ON Cohort.Id = Student.CohortId
LEFT JOIN Instructor ON Cohort.Id = Instructor.CohortId";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Cohort> cohortList = new List<Cohort>();


                    while (reader.Read())
                    {

                        Cohort currentCohortInLoop = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };

                        Student currentStudentInLoop = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Student Id")))
                        {
                            currentStudentInLoop = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Student Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("Student First Name")),
                                LastName = reader.GetString(reader.GetOrdinal("Student LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("Student SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("Student CohortId")),

                            };

                        }


                        Instructor currentInstructorInLoop = null;
                        if (!reader.IsDBNull(reader.GetOrdinal("Instructor Id")))
                        {
                            currentInstructorInLoop = new Instructor
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Instructor Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("Instructor FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("Instructor LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("Instructor SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("Instructor CohortId")),

                            };

                        }




                        // Check if cohort is already in the list
                        // If it's NOT in the list, we can add it
                        // If it is in the list, do something else 
                        if (!cohortList.Any(c => c.Id == currentCohortInLoop.Id))
                        {
                            currentCohortInLoop.StudentList.Add(currentStudentInLoop);
                            currentCohortInLoop.InstructorList.Add(currentInstructorInLoop);
                            
                            cohortList.Add(currentCohortInLoop);
                        }
                        else
                        {
                            // don't add the cohort to the list
                            Cohort cohortAlreadyInList = cohortList.Find(c => c.Id == currentCohortInLoop.Id);

                            bool instructorExists = currentInstructorInLoop != null;

                            bool instructorIsNotInList = !currentCohortInLoop.InstructorList.Any(i => i.Id == currentInstructorInLoop.Id);
                            // Check and see if the instructor is already in the cohort's instructore list 
                            // If the instructor is NOT in the list, add them
                            if (instructorExists && instructorIsNotInList)
                            {
                                cohortAlreadyInList.InstructorList.Add(currentInstructorInLoop);
                            }

                                // check and see if the student is already in the cohort's student list
                                // If NOT, add them
                            }


                    }
                    return Ok(cohortList);
                }
            }
        }

        //[HttpGet("{id}", Name = "GetCohort")]
        //public async Task<IActionResult> Get([FromRoute] int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                             SELECT 
        //            s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, c.Id AS 'Cohort Id', c.Name AS 'Cohort Name' 
        //                             FROM Cohort s
        //                             JOIN Cohort c on s.CohortId = c.Id
        //                             WHERE S.ID = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));
        //            SqlDataReader reader = cmd.ExecuteReader();

        //            Cohort cohort = null;

        //            if (reader.Read())
        //            {
        //                cohort = new Cohort
        //                {
        //                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        //                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
        //                    SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
        //                    Specialty = reader.GetString(reader.GetOrdinal("Specialty")),
        //                    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
        //                    CurrentCohort = new Cohort
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Cohort Id")),
        //                        Name = reader.GetString(reader.GetOrdinal("Cohort Name"))
        //                    }

        //                };
        //            }
        //            reader.Close();

        //            return Ok(cohort);
        //        }
        //    }
        //}

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
