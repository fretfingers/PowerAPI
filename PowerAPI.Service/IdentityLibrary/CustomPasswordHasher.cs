using Microsoft.AspNetCore.Identity;
using PowerAPI.Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Service.IdentityLibrary
{
    public class CustomPasswordHasher : IPasswordHasher<CustomIdentityUser>
    {
        public string HashPassword(CustomIdentityUser user, string password)
        {
            return EnterpriseExtras.doConvertPwd(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(CustomIdentityUser user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == EnterpriseExtras.doConvertPwd(providedPassword))
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }
    }
}
