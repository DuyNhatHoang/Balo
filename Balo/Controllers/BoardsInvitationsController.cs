using Balo.Data.ViewModels;
using Balo.Extensions;
using Balo.Service.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Balo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsInvitationsController : ControllerBase
    {
        private readonly IBoardInvitationService _bIservice;

        public BoardsInvitationsController(IBoardInvitationService bIservice)
        {
            _bIservice = bIservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetInvitationAsync(Guid? id, Guid? receiverId, Guid? senderId, int? pageIndex = 0, int? pageSize = 10)
        {
            try
            {
                var username = User.Claims.GetUserName();
                var result = await _bIservice.GetAsync(id, receiverId, senderId, pageIndex, pageSize);
                return Ok(result);


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("SentInvitations")]
        public async Task<ActionResult> GetInvitationForUserAsync()
        {

                var username = User.Claims.GetUserName();
                var result = await _bIservice.GetInvitationForUserAsync(username);
                if(result.Succeed)
                return Ok(result.Data);

                return BadRequest(result.ErrorMessage);
          
        }

        [HttpGet("ReceivedInvitations")]
        public async Task<ActionResult> GetRecievedInvitationForUserAsync()
        {

            var username = User.Claims.GetUserName();
            var result = await _bIservice.GetReceivedInvitationForUserAsync(username);
            if (result.Succeed)
                return Ok(result.Data);

            return BadRequest(result.ErrorMessage);

        }

        [HttpPost]
        public async Task<IActionResult> AddInvitationAsync([FromBody] BoardInvitationCreate model)
        {
            var username = User.Claims.GetUserName();
            var result = await _bIservice.AddAsync(username, model);

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPut]
        public async Task<IActionResult> PutInvitationStatus([FromBody] BoardInvitationUpdate model)
        {
            var username = User.Claims.GetUserName();
            var result = await _bIservice.PutInvitationStatus(username, model.Id, model.Status);

            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }
    }
}
