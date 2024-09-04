using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerAPI.Helper;
using PowerAPI.Data.IRepository;
using PowerAPI.Data.POCO;
using PowerAPI.Data.Models;
using PowerAPI.Data.ViewModels;
using PowerAPI.Service.Clients;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PowerAPI.Controllers
{
    /// <summary>
    /// Projects API endpoints
    /// </summary>
    [Authorize]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        IProjects _projects;

        /// <summary>
        /// Peoject Constructor
        /// </summary>
        /// <param name="projects"></param>
        public ProjectsController(IProjects projects)
        {
            _projects = projects;
        }

        /// <summary>
        /// gets a list of projects
        /// </summary>
        [Route("api/GetProjects/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] PaginationParams Param, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _projects.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _projects.GetProjects(Param, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.ProjectsList;
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
        /// gets a list of projects by type
        /// </summary>
        [Route("api/GetProjectsByType/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetProjectsByType([FromQuery] PaginationParams Param, string projectType, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _projects.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _projects.GetProjectsByType(Param, projectType, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.ProjectsList;
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
        /// gets project by Id
        /// </summary>
        [Route("api/GetProjectsById/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetProjectsById([FromQuery] PaginationParams Param, string Id, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _projects.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _projects.GetProjectsById(Param, Id, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.ProjectsList;
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
        /// gets a list of projects by name
        /// </summary>
        [Route("api/GetProjectsByName/{token}")]
        [HttpGet]
        public async Task<IActionResult> GetProjectsByName([FromQuery] PaginationParams Param, string name, string token)
        {
            StatusMessage statusMessage = new StatusMessage();

            var tokenObj = await _projects.GetAccess(token);

            if (tokenObj != null)
            {
                if (tokenObj.TotalDays >= 0)
                {
                    var result = await _projects.GetProjectsByName(Param, name, tokenObj);

                    statusMessage.Metadata = result.PaginationMetadata;
                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = result.ProjectsList;
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
    }
}
