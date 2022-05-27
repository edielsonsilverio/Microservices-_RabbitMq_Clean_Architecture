using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroRabbit.Transfer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _TransferService;

        public TransferController(ITransferService TransferService)
        {
            _TransferService = TransferService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TransferLog>> GetAll()
        {
            return Ok(_TransferService.GetTransferLogs());
        }

        //[HttpPost]
        //public IActionResult Post([FromBody] TransferLog TransferTransfer)
        //{
        //     var   transferTransfer = _TransferService.GetTransferLogs(TransferTransfer);
        //    return Ok(TransferTransfer);
        //}
    }
}
