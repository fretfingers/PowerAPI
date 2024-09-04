using DevExpress.XtraPrinting.Native;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.POCO;
using PowerAPI.Dto;
using PowerAPI.Service.IdentityLibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PowerAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class AppShellController : ControllerBase
    {
        private readonly IAppShellService _appShell;
        private readonly UserManager<CustomIdentityUser> _userManager;
        public AppShellController(UserManager<CustomIdentityUser> userManager, IAppShellService appShell)
        {
            _appShell = appShell;
            _userManager = userManager;
        }

        /// <summary>
        /// Endpoint for navigation links
        /// </summary>
        [HttpGet("api/navigation/{token}")]
        public async Task<ActionResult<List<NavigationDto>>> GetNavigationList(string token)
        {
            // The following list of Item Objects has been set in the TenantCredentials Middleware
            var items = HttpContext.Items;
            var apiUser = new ApiUser
            {
                CompanyId = (string)items["company"],
                DivisionId = (string)items["division"],
                DepartmentId = (string)items["department"],
                UserName = (string)items["username"],
                Roles = new List<int> { (int)items["role"] },
            };

            return await _appShell.GetNavigationInfo(apiUser);

        }


        /// <summary>
        /// Endpoint for getting list of company
        /// </summary>
        [HttpGet("api/company/{token}")]
        public async Task<ActionResult<List<string>>> GetCompanyList(string token)
        {
            // do note that token verification has beeen handled in the middleware pipeline

            var companyList =  await _appShell.GetCompany(HttpContext.User.Identity.Name);
            return Ok(new ApiResBody(200, "Sucess", "Sucess", companyList));
        }

        /// <summary>
        /// Endpoint for getting list of division
        /// </summary>
        [HttpGet("api/division/{token}")]
        public async Task<ActionResult<List<string>>> GetDivisionList(string token)
        {
            var divisionList = await _appShell.GetDivision(HttpContext.User.Identity.Name);
            return Ok(new ApiResBody(200, "Sucess", "Sucess", divisionList));
        }

        /// <summary>
        /// Endpoint for getting list of department
        /// </summary>
        [HttpGet("api/department/{token}")]
        public async Task<ActionResult<List<string>>> GetDepartmentList(string token)
        {
            var departmentList = await _appShell.GetDepartment(HttpContext.User.Identity.Name);
            return Ok(new ApiResBody(200, "Sucess", "Sucess", departmentList));
        }


        /// <summary>
        /// Endpoint for getting list of branch
        /// </summary>
        [HttpGet("api/branch/{token}")]
        public async Task<ActionResult<List<string>>> GetBranchList(string token)
        {
            var branchList = await _appShell.GetBranch(HttpContext.User.Identity.Name);
            return Ok(new ApiResBody(200, "Sucess", "Sucess", branchList));
        }


        /// <summary>
        /// Endpoint for adding Favourite navigation menu for a user
        /// </summary>
        [HttpPost("api/favourite-nav/{token}")]
        public async Task<IActionResult> AddFavouriteNav([FromBody] FavouriteNavMenuDto dto)
        {
            if (await _appShell.FavouritedMenuExists(dto))
            {
                return Conflict(new ApiException(409, "Failed", "The specified favourite menu already exists for the user."));
            }

            await _appShell.AddFavouriteMenu(dto);
            return CreatedAtAction(
                    nameof(AddFavouriteNav),
                    new { id = dto.MenuId },
                    new ApiResBody(200, "Sucess", "Sucess", dto)
                );
        }

        /// <summary>
        /// Endpoint for removing existing Favourite 
        /// </summary>
        [HttpDelete("api/favourite-nav/{token}/{companyId}/{divisionId}/{departmentId}/{username}/{menuId}")]
        public async Task<IActionResult> DeleteFavouritedMenu(string companyId, string divisionId, string departmentId, string username, int menuId)
        {
            if (string.IsNullOrWhiteSpace(companyId) || string.IsNullOrWhiteSpace(divisionId) ||
                string.IsNullOrWhiteSpace(departmentId) || string.IsNullOrWhiteSpace(username))
            {
                
                return BadRequest(new ApiException(400, "Failed", "Invalid input parameters."));
            }

            var deleted = await _appShell.DeleteFavouritedMenuAsync(companyId, divisionId, departmentId, username, menuId);

            if (!deleted)
            {
                
                return NotFound(new ApiException(404, "Failed", "Favourited menu record not found."));
            }

            return NoContent(); // 204 No Content
        }

    }
}
