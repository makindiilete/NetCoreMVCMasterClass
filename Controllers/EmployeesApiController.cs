using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp_Net_Core_Masterclass.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Asp_Net_Core_Masterclass.Controllers
{
    [AllowAnonymous]
    [Route("api/employees")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<Employee> _logger;

        public EmployeesController(IEmployeeRepository employeeRepository, ILogger<Employee> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        // GET: api/GetEmployees
        [HttpGet]
        public ActionResult GetEmployees([FromQuery] int start, int end)
        {
            try
            {
               // Employee emp = new Employee();
               var employees = _employeeRepository.GetAllEmployee().Skip(start).Take(end);
                _logger.LogWarning("Returned All Employees From The Database");
                return Ok(employees);
            }
            catch (Exception e)
            {
                _logger.LogError("Something Went Wrong Inside GetEmployees Action " + e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // GET api/employees/5
        [HttpGet("{id}")]
        public ActionResult GetEmployee(int id)
        {
            try
            {
                var employee = _employeeRepository.GetEmployee(id);
                if (employee == null)
                {
                    return StatusCode(404, $"No Employee Found With Id : {id}");
                }
                _logger.LogWarning($"Returned Employee With the Id: {id}");
                return Ok(employee);
            }
            catch (Exception e)
            {
                _logger.LogError("Something Went Wrong Inside GetEmployee Action " + e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // POST api/employees
        [HttpPost]
        public ActionResult PostEmployee([FromBody]Employee employee)
        {

            try
            {
                var newEmp =  _employeeRepository.AddEmployee(employee);
                _logger.LogWarning($"New Employee Added");
                return Ok(newEmp);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500, e.Message);
            }

        }

        // PUT api/employees/5
        [HttpPut("{id}")]
        public ActionResult UpdateEmployee(int id, [FromBody]Employee employee)
        {
            try
            {
                var emp = _employeeRepository.GetEmployee(id);
                emp.Name = employee.Name;
                emp.Email = employee.Email;
                emp.Department = employee.Department;
                _employeeRepository.UpdateEmployee(emp);
                _logger.LogWarning($"Employee Details Successfully Updated");
                return Ok(emp);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500, e.Message);
            }
        }

        // DELETE api/employees/5
        [HttpDelete("{id}")]
        public ActionResult DeleteEmployee(int id)
        {
            try
            {
                var emp = _employeeRepository.GetEmployee(id);
                if (emp == null)
                {
                    return StatusCode(404, "No Employee Found");
                }
                _employeeRepository.DeleteEmployee(id);
                _logger.LogWarning($"Employee Details Successfully Updated");
                return Ok("Employee Successfully Deleted");

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500, e.Message);
            }
        }

    }
}
