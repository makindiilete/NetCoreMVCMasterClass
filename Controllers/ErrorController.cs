using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asp_Net_Core_Masterclass.Controllers
{
    public class ErrorController : Controller
    {
        //we generate ds private field and bind it to the constructor parameter value and we will use ds field to access properties to log our errors
        private readonly ILogger<ErrorController> _logger;


        //we inject ILogger interface into our constructor for us to use it in logging error msgs in ds controller
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        //This handle http error for not found routes
        // GET
        [Route("Error/{statusCode}")] //we define our attribute route template : - "localhost:5001/Error/404"
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                //If our error code is 404
                case 404:
                    //we return this error msg
                    ViewBag.ErrorMessage = "Sorry, the resource you requested for could not be found";
                    _logger.LogWarning("404 Error Occurred. Path : {0} and Query String : {1}", statusCodeResult.OriginalPath, statusCodeResult.OriginalQueryString);

                    //we remove the following codes dt is displaying our error to users bcos we are revealing too much and dt is a security risk, instead we want to log our error
/*                    //We get the url that caused the error
                    ViewBag.Path = statusCodeResult.OriginalPath;
                    //We get the query string params in the url that caused the error.
                    ViewBag.QS = statusCodeResult.OriginalQueryString;*/
                    break;
            }
            //we then return "Notfound.cshtml"
            return View("NotFound");
        }

        //This handle any exceptions globally inside our code
        [Route("/Error")]
        [AllowAnonymous] //here we allow both authenticated and non authenticated users
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError("The path {0} threw an exception {1}", exceptionDetails.Path, exceptionDetails.Error);

                //we remove the following codes dt is displaying our error to users bcos we are revealing too much and dt is a security risk, instead we want to log our error

/*            ViewBag.ExceptionPath = exceptionDetails.Path;
            ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
            ViewBag.StackTrace = exceptionDetails.Error.StackTrace;*/

            return View("Error");
        }
    }
}
