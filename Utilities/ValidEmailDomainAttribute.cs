using System.ComponentModel.DataAnnotations;

namespace Asp_Net_Core_Masterclass.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string _allowedDomain;

        //here we generate a constructor
        public ValidEmailDomainAttribute(string allowedDomain)
        {
            _allowedDomain = allowedDomain;
        }

        //ds method takes d entered email
        public override bool IsValid(object value)
        {
            //splits d email into 2 using @ : "test@test.com" will turn ["test", "test.com"]
            string[] strings = value.ToString().Split("@");
            //then we compare the 2nd splited part ("test.com") with the custom domain we want to allow ("omoakin.com")
            return strings[1].ToUpper() == _allowedDomain.ToUpper();
        }
    }
}
