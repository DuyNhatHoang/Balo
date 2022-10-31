using Balo.Data.ViewModels;
using Balo.Extensions;
using Balo.Service.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Balo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _service;

        public TaskController(ITaskService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(CreateTaskModel model)
        {   
            var username = User.Claims.GetUserName();   
            var result = await _service.AddAsync(username, model);
            if (result.Succeed)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);

        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery]GetTaskModel model, int? pageIndex = 0, int? pageSize = 10)
        {
            var username = User.Claims.GetUserName();
            try
            {
                var result = await _service.GetPagingData(model, pageIndex, pageSize);
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
         

        }


    }
}
