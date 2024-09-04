using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.POCO;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using PowerAPI.Helper;
using Microsoft.Extensions.Options;
using PowerAPI.Dto;
using Microsoft.AspNetCore.Http;
using PowerAPI.Data.Models;
using Microsoft.AspNetCore.Identity;
using PowerAPI.Service.IdentityLibrary;
using PowerAPI.Service.Clients;
using System.Security.Cryptography;
using System.Text.Json;
//using Microsoft.AspNetCore.Authorization;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Account API endpoints
    /// </summary>
    //[Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IEmployee _employee;
        private readonly IIdGenerator _idGen;
        private readonly AppSettings _appSettings;
        private readonly UserManager<CustomIdentityUser> _userManager;


        /// <summary>
        /// account controller
        /// </summary>
        public AccountController(IEmployee employee, IOptions<AppSettings> appSettings, UserManager<CustomIdentityUser> userManager, IIdGenerator idGen)
        {
            _employee = employee;
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _idGen = idGen;
        }


        /// <summary>
        /// authenticate API token
        /// </summary>
        //[AllowAnonymous]
        [Route("api/Auth/{token}")]
        [HttpGet]
        public async Task<IActionResult> Auth(string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                
                var tokenObj = await _employee.GetAccess(token);

                if (tokenObj != null)
                {
                    if (tokenObj.TotalDays >= 0)
                    {
                       statusMessage.Status = "Success";
                       statusMessage.Message = "Success";

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
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// login - validates Username and Password for login and returns the logged in Employee record
        /// </summary>
        [Route("api/Login/{token}")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login( string token, [FromBody] LoginDto loginDto)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (loginDto.Username == null || loginDto.Username == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Username";
                    return Unauthorized(statusMessage);
                }
                else if (loginDto.Password == null || loginDto.Password == "")
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Enter A Valid Password";

                    return Unauthorized(statusMessage);
                }
                else
                {
                    var tokenObj = await _employee.GetAccess(token);


                    var uniqueName = _idGen.GenerateId(tokenObj.CompanyId, tokenObj.DivisionId, tokenObj.DepartmentId, loginDto.Username);   

                    var user = await _userManager.FindByNameAsync(uniqueName);

                    if(user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                    {
                        statusMessage.Status = "Failed";
                        statusMessage.Message = "Incorrect Username and/or Password.";

                        return Unauthorized(new
                        {
                            StatusCode = 401,
                            Status = "Failed",
                            Message = "Incorrect Username and/or Password.",
                        });
                    }

                    var jWtToken = await GenerateJWT(uniqueName);

                    var refreshToken = GenerateRefreshToken();

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);

                    await _userManager.UpdateAsync(user);


                    var data = new { UserId = user.Id,
                        user.UserName,
                        location = new
                        {
                            company = user.CurrentCompany,
                            division = user.CurrentDivision,
                            department = user.CurrentDepartment,
                            branch = user.CurrentBranch,
                        },
                        AccType = new List<string>{ "user" },
                        user.Warehouses,
                    }; 

                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Success",
                        Data = data,
                        JwtToken = new JwtSecurityTokenHandler().WriteToken(jWtToken),
                        Expiration = jWtToken.ValidTo,
                        RefreshToken = refreshToken
                    });

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieve new JWT Access Token using Refreh Token
        /// </summary>
        [HttpPost("api/Refresh/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto model)
        {
            try { 

            var principal = GetPrincipalFromExpiredToken(model.AccessToken);

            if (principal?.Identity?.Name is null)
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user is null || user.RefreshToken != model.RefreshToken)
                return Unauthorized();

            if (user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized("Refresh token expired");
                

            var token = await GenerateJWT(principal.Identity.Name);

            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);


            return Ok(new
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                RefreshToken = refreshToken
                });
            } catch
            {
                return BadRequest(new
                {
                    Message = "Failed, Please check that request is properly formatted."

                });
            }
        }



        /// <summary>
        /// Revoke Access.
        /// </summary>
        [Authorize]
        [HttpDelete("api/Logout/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Revoke()
        {

            var username = HttpContext.User.Identity?.Name;

            if (username is null)
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
                return Unauthorized();

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return Ok("Access revocation request successful");
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {try
            {


                var secret = _appSettings.Secret ?? throw new InvalidOperationException("Secret not configured");

                var validation = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateLifetime = false,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                };

                return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
            }
            catch { throw; }
        }

        //private string GenerateToken(string Username)
        //{
        //    try
        //    {
                

        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(new Claim[]
        //            {
        //              new Claim(ClaimTypes.Name, Username.ToString())
        //            }),

        //            //Expires = DateTime.UtcNow.AddMinutes(60),
        //            Expires = DateTime.UtcNow.AddSeconds(100),
        //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //        };
        //        var signedToken = tokenHandler.CreateToken(tokenDescriptor);
        //        var tokenString = tokenHandler.WriteToken(signedToken);

        //        return tokenString;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
            
        //}

        private async Task<JwtSecurityToken> GenerateJWT(string Username)
        {

            var user = await _userManager.FindByNameAsync(Username);

            if (user == null) throw new Exception("user not found");


            //var Location = new
            //{
            //    CurrentComp = user.CurrentCompany ?? defaultComp,
            //    CurrentDiv = user.CurrentDivision ?? defaultDiv,
            //    CurrentDept = user.CurrentDepartment ?? defaultDept,
            //    CurrentSite = user.CurrentBranch ?? defaultSite,
            //};
            //var locationString = JsonSerializer.Serialize(Location);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Role", user.Roles[0].ToString())
                //new Claim("Location", locationString),
            };
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var signedToken = new JwtSecurityToken(     
                    claims: authClaims,
                    expires: DateTime.UtcNow.AddMinutes(15),
 
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                );


                return signedToken;
            }
            catch (Exception)
            {
                return null;
            }

        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var generator = RandomNumberGenerator.Create();

            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }


    }
}
