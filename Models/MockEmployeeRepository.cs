using System;
using System.Collections.Generic;
using System.Linq;

namespace Asp_Net_Core_Masterclass.Models
{
    //This class will provide the implementation for the IEmployeeRepository so it inherits from the IEmployeerepo
    public class MockEmployeeRepository : IEmployeeRepository
    {
        //We create a private field which will hold the list of our employees
        private List<Employee> _employeeList;

        //We create a constructor and inside it we set the value of our "_employeeList" to employee objects
        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>
            {
                new Employee {Id = 1, Name = "John", Email = "john@gmail.com", Department = DeptEnum.HR},
                new Employee {Id = 2, Name = "Michael", Email = "mike@gmail.com", Department = DeptEnum.IT},
                new Employee {Id = 3, Name = "Rock", Email = "rock@gmail.com", Department = DeptEnum.Payroll}
            };
        }

        //Here we provide implementation for the GetEmployee() from the IEmployeeRepository interface
        public Employee GetEmployee(int id)
        {
            //using lambda expression, we filter the employee list and return the employee whose id matches the id in the Getemployee() parameter
            return _employeeList.FirstOrDefault(e => e.Id == id);
        }

        //we are providing implementation for the GetAllEmployee() method..
        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeList;
        }


        //We are providing implementation for the AddEmployee method
        public Employee AddEmployee(Employee employee)
        {
            //Here we add the passed employee object parameter to the existing employee list and return the employee added
         employee.Id =   _employeeList.Max(e => e.Id + 1); //Here we add 1 to the maximum value of the id field in the list in other to create an Id for the new employee...
             _employeeList.Add(employee);
             return employee;
        }

        public Employee UpdateEmployee(Employee employeeChanges)
        {
            //We check the list of employee for an employee whose id matches the id value of the passed "employeeChanges" parameter
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == employeeChanges.Id);
            //if a matching employee is found
            if (employee != null)
            {
                //we update the details of the found employee with the details contained in the employeeChanges
                employee.Name = employeeChanges.Name;
                employee.Email = employeeChanges.Email;
                employee.Department = employeeChanges.Department;
            }
            //we then return the updated employee
            return employee;
        }

        public Employee DeleteEmployee(int id)
        {
            //We check the list to find the employee whose id matches the supplied id parameter
            Employee employee = _employeeList.FirstOrDefault(e => e.Id == id);
            //if we find a matching employee
            if (employee != null)
            {
                //we remove the employee from the list
                _employeeList.Remove(employee);
            }
            //we return the removed employee
            return employee;
        }

    }
}
