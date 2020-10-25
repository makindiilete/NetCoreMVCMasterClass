using System.Collections;
using System.Collections.Generic;

namespace Asp_Net_Core_Masterclass.Models
{
    public interface IEmployeeRepository
    {
        //Here we have a method to call to get an employee by their Id
        Employee GetEmployee(int id);

        //Here we have a method to get the list of all employees and since we are dealing with a collection, we use IEnumerable<> type..
        IEnumerable<Employee> GetAllEmployee();

        //Here we have a method to create/add a new employee
        Employee AddEmployee(Employee employee);

        //here we have a method to update employee details
        Employee UpdateEmployee(Employee employeeChanges);

        //Here we have a method to delete an employee
        Employee DeleteEmployee(int id);

    }
}
