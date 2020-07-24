// ExportController.cs is part of LiftLog and was created on 04/14/2017. 
// Last modified on 04/15/2017.

using System;
using System.Threading.Tasks;
using LiftLog.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LiftLog.Web.Controllers.Api
{
    /// <summary>
    ///     web controller for export
    /// </summary>
    [Route("api/[controller]")]
    public class ExportController : Controller
    {
        private readonly IExportService _exportService;

        private readonly ILogger _logger;

        public ExportController(ILoggerFactory logger, IExportService exportService)
        {
            _logger = logger.CreateLogger<ExportController>();
            _exportService = exportService;
        }


        /// <summary>
        ///     Returns all user data in json with a unqiue token
        ///     please note that the token is refreshed for each call, thus one is required to retrieve a new token
        ///     after each use.
        /// </summary>
        /// <param name="token">unique token</param>
        /// <returns>json of all user data for a specific user</returns>
        [HttpGet]
        public async Task<IActionResult> GetUser(string token)
        {
            try
            {
                if (!Guid.TryParse(token, out Guid parsedToken))
                    return BadRequest();

                var dto = await _exportService.ExportToPortableObjectAsync(parsedToken);
                if (dto == null)
                    return Forbid();
                return Json(dto);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}