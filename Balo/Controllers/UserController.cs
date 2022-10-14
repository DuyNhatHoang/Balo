using Balo.Data.MongoCollections;
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
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Add([FromBody] CreateUserModel model)
        {
            var userName = User.Claims.GetUserName();
            var result = await _service.AddAsync(userName, model);

            if (result.Succeed)
            {
                return CreatedAtAction(nameof(Get), new { id = result.Data }, model);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("id")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.Get(id);

            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string? name, Guid? id, int? pageIndex = 0, int? pageSize = 10)
        {
            try
            {
                var result = await _service.GetPagingData(name, id, pageIndex, pageSize);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserModel model)
        {
            try
            {
                await _service.Update(id, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }
    }
}
