
using System;
using System.IO;
using System.Linq;
using Asp_Net_Core_Masterclass.Models;
using Asp_Net_Core_Masterclass.Security;
using Asp_Net_Core_Masterclass.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Asp_Net_Core_Masterclass.Controllers
{
    public class HomeController : Controller
    {
        //This fields contains all the methods/logic to manage employee data
        private readonly IEmployeeRepository _employeeRepository;
        //This fields is used to get the path to the uploaded employee photo
        private readonly IHostingEnvironment _hostingEnvironment;
        //this field will be used to decrypt user Ids in route url
        private readonly IDataProtector _dataProtectionProvider;


        //Injecting our interface into HomeController constructor with dependency injection and naming the var "employeeRepository", the 2nd parameter "hostingEnvironment is injected to get the path of the photo upload
        public HomeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings )
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
            //Needed for encrypting our route id
            _dataProtectionProvider =
                dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }


        [HttpGet]
        [AllowAnonymous] //we allow anonymous to be able to view d index page
        public ViewResult Index()
        {
            //We create a variable to store the model data dt contain a list of all employees
            var model = _employeeRepository.GetAllEmployee()
                .Select(e =>
                {
                    // Encrypt the ID value and store in EncryptedId property
                    e.EncryptedId = _dataProtectionProvider.Protect(e.Id.ToString());
                    return e;
                });
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous] //we allow anonymous to be able to view d index page
        public ViewResult Details(string id)//we receive the encrypted id as params
        {

            //we decryt the id and convert it to integer.
            int employeeId = Convert.ToInt32(_dataProtectionProvider.Unprotect(id));

            //We first check if the employee we looking for exist
            Employee employee = _employeeRepository.GetEmployee(employeeId);
            if (employee == null)
            {
                //if we cant find the employee we set response code to 404
                Response.StatusCode = 404;
                //we redirect user to "EmployeeNotFound page and pass the value user enters also so we can display it in the view.
                return View("EmployeeNotFound", employeeId);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };

            //To use strongly typed View, we pass our model directly to the View() method
            return View(homeDetailsViewModel);
        }

        //This action method returns the create employee form when we issue a get request to the url https://localhost:5001/Home/Create

        [HttpGet] //We want this version of Create() to respond to Http get request
        public ViewResult Create()
        {
            return View();
        }

        //This action method handle POST request to create a new employee with uploaded image to the url https://localhost:5001/Home/Create..
        //it takes an employee object as parameter
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {

            //If the http request body is not valid, we return back the view for the client to correct the invalid fields
            if (!ModelState.IsValid) return View();
            string uniqueFileName = null;
            //if d statement is not null, it means users has selected a file
            if (model.Photo != null)
            {
                //To set our uploadsFolder, we use "Path.Combine(_hostingEnvironment.WebRootPath, "images");"
                //_hostingEnvironment.WebRootPath gives us path to "wwwroot", since we want the "images" folder under it, we use the "Path.Combine" to combine/attach the "images" subfolder to it
             string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

             //This ensure each uploaded file contain a unique file name by adding a unique id to their file name
           uniqueFileName =  Guid.NewGuid().ToString() + "_" + model.Photo.FileName;

           //combining our uploadsFolder and uniqueFileName gives us our filePath
           string filePath = Path.Combine(uploadsFolder, uniqueFileName);

           //Now we copy the file to the wwwroot/images folder using the "CopyTo()" of the IFormFile Type.. Ds takes the filePath and the "FileMode.Create" which tells the OS to create the file
           model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            //Now we need to add this new employee using the properties in the Employee model and the value for each properties is in the body of the post request which is the parameter of this action method
            Employee newEmployee = new Employee
            {
                Name = model.Employee.Name,
                Email = model.Employee.Email,
                Department = model.Employee.Department,
                PhotoPath = uniqueFileName
            };
            _employeeRepository.AddEmployee(newEmployee);
            return RedirectToAction("Details", new {id = newEmployee.Id});
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            //We call the GetEmployee() method on the interface and pass d id...So the employee details is stored inside employee
            Employee employee = _employeeRepository.GetEmployee(id);
            //With the retrieved details, we store each property values inside the properties of the "employeeEditViewModel"
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

                //This action method handle POST request to update existing employee details
        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {

            //If the http request body is not valid, we return back the view for the client to correct the invalid fields
            if (!ModelState.IsValid) return View();

            //We start by retrieving existing details of the employee
            Employee employee = _employeeRepository.GetEmployee(model.Id);
            employee.Name = model.Name;
            employee.Department = model.Department;
            employee.Email = model.Email;

            //File Upload processing...

          //if the user select a new photo i.e. they choose to change existing photo
            if (model.Photo != null)
            {
                //We check if the user is having existing photo
                if (model.ExistingPhotoPath != null)
                {
                    //we get the path to the photo
                string existingPhotoPathToDelete =  Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);

                //then we call the Delete() method from "system.io.file" to delete the file
                System.IO.File.Delete(existingPhotoPathToDelete);
                }
                //To set our uploadsFolder, we use "Path.Combine(_hostingEnvironment.WebRootPath, "images");"
                //_hostingEnvironment.WebRootPath gives us path to "wwwroot", since we want the "images" folder under it, we use the "Path.Combine" to combine/attach the "images" subfolder to it
             string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

             //This ensure each uploaded file contain a unique file name by adding a unique id to their file name
           var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;

           //combining our uploadsFolder and uniqueFileName gives us our filePath
           string filePath = Path.Combine(uploadsFolder, uniqueFileName);

           //Now we copy the file to the wwwroot/images folder using the "CopyTo()" of the IFormFile Type.. Ds takes the filePath and the "FileMode.Create" which tells the OS to create the file
           model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

           //We set the photoPath property to the new selected photo
           employee.PhotoPath = uniqueFileName;
           //We call the "UpdateEmployee" method and pass the updated details
            }
            //If user choose not to change their photo
            else if (model.Photo == null)
            {
                //We set the photoPath to the existing photo which we have inside the "ExistingPhotoPath" property of our model.
                employee.PhotoPath = model.ExistingPhotoPath;
            }

            _employeeRepository.UpdateEmployee(employee);
            //we redirect the user to index page.
            return RedirectToAction("Index");
        }
    }
}


