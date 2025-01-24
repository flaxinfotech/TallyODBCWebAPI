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

        // Add or update a ledger using ODBC
        [HttpPost("ledger/odbc")]
        public async Task<IActionResult> AddOrUpdateLedgerOdbc([FromBody] LedgerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _odbcService.AddOrUpdateLedgerAsync(
                    request.LedgerName,
                    request.ParentGroup,
                    request.Address,
                    request.Email,
                    request.IsUpdate
                );

                return Ok(new { Message = result });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // Add or update a ledger using XML
        [HttpPost("ledger/xml")]
        public async Task<IActionResult> AddOrUpdateLedgerXml([FromBody] LedgerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _xmlService.AddOrUpdateLedgerAsync(
                    request.LedgerName,
                    request.ParentGroup,
                    request.Address,
                    request.Email
                );

                return Ok(new { Message = "Ledger processed successfully via XML.", Response = result });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // Get ledgers using ODBC
        [HttpGet("ledgers/odbc")]
        public async Task<IActionResult> GetLedgersOdbc()
        {
            try
            {
                var ledgers = await _odbcService.GetLedgersAsync();
                return Ok(ledgers);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // Get ledgers using XML
        [HttpGet("ledgers/xml")]
        public async Task<IActionResult> GetLedgersXml()
        {
            try
            {
                var result = await _xmlService.GetLedgersXmlAsync();
                return Ok(new { Message = "Ledger list fetched successfully via XML.", Response = result });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

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
