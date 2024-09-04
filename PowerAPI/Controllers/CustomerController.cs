using DevExpress.XtraPrinting.Native.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Data.ViewModels;
using PowerAPI.Helper;
using PowerAPI.Service.Clients;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Account API endpoints
    /// </summary>
    //[Authorize]
    [ApiController]
    public class CustomerController : ControllerBase
    {
            ICustomer _customer;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// customer controller
        /// </summary>
        public CustomerController(ICustomer customer, IOptions<AppSettings> appSettings)
        {
            _customer = customer;
            _appSettings = appSettings.Value;
        }

        ///// <summary>
        ///// authenticate API token
        ///// </summary>
        ////[AllowAnonymous]
        //[Route("api/Auth/{token}")]
        //[HttpGet]
        //public async Task<IActionResult> Auth(string token)
        //{
        //    StatusMessage statusMessage = new StatusMessage();

        //    try
        //    {

        //        var tokenObj = await _customer.GetAccess(token);

        //        if (tokenObj != null)
        //        {
        //            if (tokenObj.TotalDays >= 0)
        //            {
        //                statusMessage.Status = "Success";
        //                statusMessage.Message = "Success";

        //                return Ok(statusMessage);
        //            }
        //            else
        //            {
        //                statusMessage.Status = "Failed";
        //                statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

        //                return Ok(statusMessage);
        //            }
        //        }
        //        else
        //        {
        //            statusMessage.Status = "Failed";
        //            statusMessage.Message = "Invalid Token";

        //            return Ok(statusMessage);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// login - validates Username and Password for custozcmer login and returns the logged in customer record by id
        /// </summary>
        [Route("api/CustomerLogin/{token}")]
        [HttpGet]
        public async Task<IActionResult> CustomerLogin(string Username, string Password, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                if (Username == null || Username == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Username";
                    return Ok(statusMessage);
                }
                else if (Password == null || Password == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Password";
                    return Ok(statusMessage);
                }
                else
                {
                    var tokenObj = await _customer.GetAccess(token);
                    if (tokenObj != null)
                    {
                        if (tokenObj.TotalDays >= 0)
                        {
                            bool status = await _customer.CustomerLogin(Username, Password, tokenObj);

                            if (status)
                            {
                                //get employee by id
                                var employee = await _customer.GetCustomerById(Username, tokenObj); 

                                //generate access token
                                var tokenString = generateToken(Username);

                                statusMessage.Status = "Success";
                                statusMessage.Message = "Success";
                                statusMessage.data = employee;
                                statusMessage.auth_token = tokenString;

                                return Ok(statusMessage);
                            }
                            if (status)
                            {
                                //get employee by email
                                var employee = await _customer.GetCustomerByEmail(Username, tokenObj);

                                //generate access token
                                var tokenString = generateToken(Username);

                                statusMessage.Status = "Success";
                                statusMessage.Message = "Success";
                                statusMessage.data = employee;
                                statusMessage.auth_token = tokenString;

                                return Ok(statusMessage);
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Incorrect Username and/or Password.";

                                return Ok(statusMessage);
                            }
                            
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                            return Ok(statusMessage);
                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Token";

                        return Ok(statusMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Change Employee Password
        /// </summary>
        /// <param name="changePassword"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("api/ChangePasswordCustomer/{token}")]
        [HttpPost]
        public async Task<IActionResult> ChangePasswordCustomer(PasswordModel changePassword, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (changePassword == null)
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Invalid Employee Change Data";
                    return Ok(statusMessage);
                }
                else if (changePassword.Username == null || changePassword.Username == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Username Cannot Be Null/Empty.";
                    return Ok(statusMessage);
                }
                else if (changePassword.OldPassword == null || changePassword.OldPassword == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Old Password Cannot Be Null/Empty.";
                    return Ok(statusMessage);
                }
                else if (changePassword.NewPassword == null || changePassword.NewPassword == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "New Password Cannot Be Null/Empty.";
                    return Ok(statusMessage);
                }
                else
                {
                    Regex regexObj = new Regex(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{2,})$");
                    bool isMatch = changePassword.NewPassword == null || changePassword.NewPassword == "" ? false : regexObj.IsMatch(changePassword.NewPassword);

                    if (changePassword.NewPassword.Length < 8)
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Password Must Be Minimum Of Eight (8) Characters";
                        return Ok(statusMessage);
                    }
                    else //if(isMatch) this will be revisited
                    {
                        var tokenObj = await _customer.GetAccess(token);

                        if (tokenObj != null)
                        {
                            if (tokenObj.TotalDays >= 0)
                            {
                                statusMessage = await _customer.ChangePwd(changePassword, tokenObj);
                                return Ok(statusMessage);
                            }
                            else
                            {
                                statusMessage.Status = "Failed";
                                statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                                return Ok(statusMessage);
                            }
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Invalid Token";

                            return Ok(statusMessage);
                        }

                    }
                    //else // this will be un-commented when the checks for alpha-numeric is closed/dealt
                    //{
                    //    statusMessage.Status = "Failed";
                    //    statusMessage.Message = "Password Must Be AlphaNumeric. Must Contain Numbers, Alphabets And Characters.";
                    //    return Ok(statusMessage);
                    //}

                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Unknown Error. Try Again";

                return Ok(statusMessage);
            }
        }

        /// <summary>
        /// login - validates Username and Password for customer login and returns the logged in customer record by email
        /// </summary>
        [Route("api/CustomerLoginEmail/{token}")]
        [HttpGet]
        public async Task<IActionResult> CustomerLoginEmail(string Username, string Password, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                if (Username == null || Username == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Username";
                    return Ok(statusMessage);
                }
                else if (Password == null || Password == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Password";
                    return Ok(statusMessage);
                }
                else
                {
                    var tokenObj = await _customer.GetAccess(token);
                    if (tokenObj != null)
                    {
                        if (tokenObj.TotalDays >= 0)
                        {
                                var cust = await _customer.CustomerLoginEmail(Username, Password, tokenObj);

                                if (cust != null)
                                {
                                    
                                    //generate access token
                                    var tokenString = generateToken(Username);

                                    statusMessage.Status = "Success";
                                    statusMessage.Message = "Success";
                                    statusMessage.data = cust;
                                    statusMessage.auth_token = tokenString;

                                }
                                else
                                {
                                    statusMessage.Status = "Failed";
                                    statusMessage.Message = "Incorrect Username and/or Password.";

                                }

                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Token";

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Ok(statusMessage);
        }

        /// <summary>
        /// reset password - reset user password
        /// </summary>
        [Route("api/ResetPasswordOTP/{token}")]
        [HttpGet]
        public async Task<IActionResult> ResetPasswordOTP(string Username, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (Username == null || Username == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Username";
                    return Ok(statusMessage);
                }
                else
                {
                    var tokenObj = await _customer.GetAccess(token);

                    if (tokenObj != null)
                    {
                        if (tokenObj.TotalDays >= 0)
                        {
                            var result = await _customer.ResetPasswordOTP(Username, tokenObj);

                            statusMessage = result;

                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Token";
                    }
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Unknown Error. Try Again";

            }
            return Ok(statusMessage);
        }

        /// <summary>
        /// Validates Password - Checks if OTP is valid
        /// </summary>
        [Route("api/ValidateOTP/{token}")]
        [HttpGet]
        public async Task<IActionResult> ValidateOTP(string Username, string OTP, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (Username == null || Username == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Username";
                    return Ok(statusMessage);
                }
                else
                {
                    var tokenObj = await _customer.GetAccess(token);

                    if (tokenObj != null)
                    {
                        if (tokenObj.TotalDays >= 0)
                        {
                            var result = await _customer.ValidateOTP(Username, OTP, tokenObj);

                            statusMessage = result;
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";
                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Token";
                    }
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;

            }
            return Ok(statusMessage);
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        [Route("api/ResetPassword/{token}")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string Username, string OTP, string Password, string token)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                if (Username == null || Username == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Username";
                    return Ok(statusMessage);
                }
                else if(OTP == null || OTP == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid OTP";
                    return Ok(statusMessage);
                }
                else if (Password == null || Password == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Password";
                    return Ok(statusMessage);
                }
                else
                {
                    var tokenObj = await _customer.GetAccess(token);

                    if (tokenObj != null)
                    {
                        if (tokenObj.TotalDays >= 0)
                        {
                            var result = await _customer.ResetPassword(Username, OTP, Password, tokenObj);

                            statusMessage = result;
                        }
                        else
                        {
                            statusMessage.Status = "Failed";
                            statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";
                        }
                    }
                    else
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Invalid Token";
                    }
                }
            }
            catch(Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }
            return Ok(statusMessage);
        }


        /// <summary>
        /// gets a list of customer
        /// </summary>
        [Route("api/GetCustomer/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetCustomer([FromQuery] PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _customer.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _customer.GetCustomer(Param, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.CustomerList;
                    return Ok(statusMessage);
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                    return BadRequest(statusMessage);
                }
            }
            else
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Invalid Token";

                return BadRequest(statusMessage);
            }

        }

        /// <summary>
        /// gets a list of sales representative
        /// </summary>
        [Route("api/GetSalesRepresentatives/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetSalesRepresentatives([FromQuery] PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _customer.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _customer.GetSalesRepresentatives(Param, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.SalesRepresentativesList;
                    return Ok(statusMessage);
                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

                    return BadRequest(statusMessage);
                }
            }
            else
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = "Invalid Token";

                return BadRequest(statusMessage);
            }

        }


        ///// <summary>
        ///// generate Otp 
        ///// </summary>
        //[Route("api/generateOTP/{token}")]
        //[HttpGet]
        //public string generateOTP(string token)
        //{
        //    StatusMessage statusMessage = new StatusMessage();
        //    OTPGeneratorCode oTPGenerator = new OTPGeneratorCode();
        //    var randomCode = "";
        //    try
        //    {
        //        Random random = new Random();
        //        randomCode = (random.Next(999999).ToString());
        //        return randomCode;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


        private string generateToken(string Username)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                      new Claim(ClaimTypes.Name, Username.ToString())
                    }),

                    //Expires = DateTime.UtcNow.AddMinutes(60),
                    Expires = DateTime.UtcNow.AddDays(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var signedToken = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(signedToken);

                return tokenString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///// <summary>
        ///// gets Order record by Customer Id
        ///// </summary>
        //[Route("api/GetOrderByCustomer/{token}")]
        //[HttpGet]
        //public async Task<IActionResult> GetOrderByCustomer(string CustomerId, string token)
        //{
        //    StatusMessage statusMessage = new StatusMessage();

        //    try
        //    {
        //        var tokenObj = await _customer.GetAccess(token);

        //        if (tokenObj != null)
        //        {
        //            if (tokenObj.TotalDays >= 0)
        //            {
        //                var employee = await _customer.GetOrderByCustomer(CustomerId, tokenObj);

        //                statusMessage.Status = "Success";
        //                statusMessage.Message = "Success";
        //                statusMessage.data = employee;
        //                return Ok(statusMessage);
        //            }
        //            else
        //            {
        //                statusMessage.Status = "Failed";
        //                statusMessage.Message = "Access Denied. License Expired. Contact System Administrator";

        //                return Ok(statusMessage);
        //            }
        //        }
        //        else
        //        {
        //            statusMessage.Status = "Failed";
        //            statusMessage.Message = "Invalid Token";

        //            return Ok(statusMessage);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        statusMessage.Status = "Failed";
        //        statusMessage.Message = "Unknown Error. Try Again";

        //        return Ok(statusMessage);
        //    }
        //}
    }
    
}
