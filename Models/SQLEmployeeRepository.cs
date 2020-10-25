using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Asp_Net_Core_Masterclass.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public SQLEmployeeRepository(AppDbContext context)
        {
            _context = context;
        }
        public Employee GetEmployee(int id)
        {
            return _context.Employees.Find(id);
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _context.Employees;
        }

        public Employee AddEmployee(Employee employee)
        {
            //We first check if a user with the given email already exist before creating the user
            var doesEmployeeExist = GetAllEmployee().FirstOrDefault(e => e.Email == employee.Email);
            if (doesEmployeeExist != null && doesEmployeeExist.Email != null)
            {
               throw new Exception("User Already Exist");
            }
            //We use the "_context" private field to add the new employee
            _context.Employees.Add(employee);
            //we call the SaveChanges() to save the changes to the database
            _context.SaveChanges();
            //we return the added employee..
            return employee;
        }


        public Employee UpdateEmployee(Employee employeeChanges)
        {
            var employee = _context.Employees.Attach(employeeChanges);
            employee.State = EntityState.Modified;
            _context.SaveChanges();
            return employeeChanges;
        }

        public Employee DeleteEmployee(int id)
        {
            //We find the employee with the passed id in the database
           Employee employee = _context.Employees.Find(id);
           //if we find a match
           if (employee != null)
           {
               //we remove the employee
               _context.Employees.Remove(employee);
               //we save our changes
               _context.SaveChanges();
           }
            //we return the removed employee
           return employee;
        }
    }
}
