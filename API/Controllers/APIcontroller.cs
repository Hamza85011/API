using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using API.Model;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIcontroller : ControllerBase
    {
        private readonly ILogger<APIcontroller> _logger;
        private readonly string _connectionString;

        public APIcontroller(ILogger<APIcontroller> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("cs");
        }

        [HttpGet]       
        public List<Student> GetList()
        {
            List<Student> users = new List<Student>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_Get", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    Student user = new Student
                    {
                        Name = Convert.ToString(dr["Name"]),
                        Position = Convert.ToString(dr["Position"]),
                        Department = Convert.ToString(dr["Department"]),
                        Id = Convert.ToInt32(dr["Id"])
                    };
                    users.Add(user);
                }
            }
            return users;
        }

        [HttpPost]
        public IActionResult Create(Student model)
        {
            List<Student> users = new List<Student>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_Create", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", model.Id);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Position", model.Position);
                cmd.Parameters.AddWithValue("@Department", model.Department);
                con.Open();
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                {
                    return Ok(new { Message = "Employee data Created successfully" });
                }
                else
                {
                    return NotFound(new { Message = "Employee not found" });
                }
            }
        }
        [HttpPut]
        public IActionResult Update(Student model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Position", model.Position);
                cmd.Parameters.AddWithValue("@Department", model.Department);
                cmd.Parameters.AddWithValue("@Id", model.Id);
                con.Open();
                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                {
                    return Ok(new { Message = "Employee data updated successfully" });
                }
                else
                {
                    return NotFound(new { Message = "Employee not found" });
                }
            }
        }
        [HttpGet ("{id}")]
        public Student GetDetails(int id)
        {
            Student model = new Student();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_Details", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataAdapter adapterr = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapterr.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    model.Id = Convert.ToInt32(dr["Id"]);
                    model.Name = Convert.ToString(dr["Name"]);
                    model.Position = Convert.ToString(dr["Position"]);
                    model.Department = Convert.ToString(dr["Department"]);
                }
            }
            return model;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_Delete", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return Ok(new { Message = "Employee deleted successfully" });
                }
                else
                {
                    return NotFound(new { Message = "Employee not found" });
                }
            }
        }

    }
}
