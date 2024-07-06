using Clipess.DBClient.EntityModels;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Clipess.DBClient.Contracts;

namespace Clipess.API.Controllers;

[Route("api/maritalStatus")]
[ApiController]
public class MaritalStatusControllercs : ControllerBase
{
    private static ILog _logger;
    private readonly IMaritalStatusRepository _maritalStatusRepository;

    public MaritalStatusControllercs(IMaritalStatusRepository maritalStatusRepository)
    {
        _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        _maritalStatusRepository = maritalStatusRepository;

    }

    [Route("getMaritalStatus")]
    [HttpGet]
    public async Task<ActionResult> GetMaritalStatus([FromQuery] int MaritalStatusID)
    {
        {
            try
            {
                var maritalStatus = _maritalStatusRepository.GetMaritalStatus();
                if (maritalStatus != null)
                {
                    return Ok(maritalStatus);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error($"An error occurred in: {nameof(GetMaritalStatus)} for MaritalStatusID: {MaritalStatusID}, exception: {ex.Message}.");
                return BadRequest();
            }
        }
    }

    [HttpPost]
    [Route("addMaritalStatus")]
    public async Task<IActionResult> AddMaritalStatus([FromBody] MaritalStatus recevingMaritalStatus)
    {
        try
        {
            var maritalStatus = new MaritalStatus
            {
                MaritalStatusID = recevingMaritalStatus.MaritalStatusID,
                MaritalStatusType = recevingMaritalStatus.MaritalStatusType,
            };

            await _maritalStatusRepository.AddMaritalStatus(maritalStatus);
            return Ok(maritalStatus);
        }
        catch (Exception ex)
        {
            _logger.Error($"An error occurred in: {nameof(GetMaritalStatus)} , exception: {ex.Message}.");
            return BadRequest();
        }
    }
}
