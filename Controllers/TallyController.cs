using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TallyIntegrationAPI.Models;
using TallyIntegrationAPI.Services;

namespace TallyIntegrationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TallyController : ControllerBase
    {
        private readonly OdbcService _odbcService;
        private readonly XmlService _xmlService;

        public TallyController(OdbcService odbcService, XmlService xmlService)
        {
            _odbcService = odbcService;
            _xmlService = xmlService;
        }

        [HttpPost("create-ledger")]
        public async Task<IActionResult> CreateLedger([FromBody] LedgerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LedgerName) || string.IsNullOrWhiteSpace(request.ParentGroup))
            {
                return BadRequest("Ledger Name and Parent Group are required.");
            }

            string result = await _xmlService.SendLedgerToTallyAsync(request);

            if (result.Contains("Success"))
            {
                return Ok(new { Message = result });
            }
            else
            {
                return StatusCode(500, new { Error = result });
            }
        }

        [HttpGet("get-all-ledgers")]
        public async Task<IActionResult> GetAllLedgers()
        {
            try
            {
                var ledgers = await _odbcService.GetAllLedgersAsync();

                if (ledgers.Count > 0)
                {
                    return Ok(ledgers);
                }
                else
                {
                    return NotFound(new { Message = "No ledgers found in Tally." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        //// Get ledgers using XML
        //[HttpGet("ledgers/xml")]
        //public async Task<IActionResult> GetLedgersXml()
        //{
        //    try
        //    {
        //        var result = await _xmlService.GetLedgersXmlAsync();
        //        return Ok(new { Message = "Ledger list fetched successfully via XML.", Response = result });
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return StatusCode(500, new { Error = ex.Message });
        //    }
        //}

        [HttpGet("ledgers/advanced")]
        public async Task<IActionResult> GetLedgersWithFilters(
            [FromQuery] string ledgerName,
            [FromQuery] string parentGroup,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var ledgers = await _odbcService.GetFilteredLedgersAsync(ledgerName, parentGroup, startDate, endDate);
                return Ok(ledgers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }
}
