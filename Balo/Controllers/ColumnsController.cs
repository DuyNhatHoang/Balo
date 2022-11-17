using Balo.Data.ViewModels;
using Balo.Extensions;
using Balo.Service.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Balo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ColumnsController : Controller
    {
        private readonly IColumnService _service;

        public ColumnsController(IColumnService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupForBoard([FromBody] CreateColumnModel model)
        {
            var username = User.Claims.GetUserName();
            var result = await _service.AddAsync(username, model);
            if (result.Succeed)
                return Ok(result.Data);

            return BadRequest(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<IActionResult> GetActionResultAsync([FromQuery] GetColumnModel model, int? pageIndex = 0, int? pageSize = 10)
        {
            var username = User.Claims.GetUserName();
            try
            {
                var result = await _service.GetAsync(username, model, pageIndex, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

}
}
