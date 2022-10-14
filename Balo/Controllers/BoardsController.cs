using Balo.Data.MongoCollections;
using Balo.Data.ViewModels;
using Balo.Extensions;
using Balo.Service.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Balo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly IBoardService _service;

        public BoardsController(IBoardService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddBoardAsync([FromBody] BoardCreateModel model)
        {
            var result = await _service.AddAsync(model);

            if(result.Succeed)
            {
                return CreatedAtAction(nameof(Get), new { id = result.Data }, model);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.Get(id);

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetPagingData(int? pageIndex = 0, int? pageSize = 10)
        {
            try {    
                var username = User.Claims.GetUserName();
                var rs = await _service.GetPagingData(pageIndex, pageSize);
                return Ok(rs);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

           
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, [FromBody] Board model)
        {
            try
            {
                await _service.Update(id, model);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}
