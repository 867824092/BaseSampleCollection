using AspNetCoreIntegration.Data;
using AspNetCoreIntegration.Binders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIntegration.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class BoundAuthorsController : ControllerBase {
        #region snippet_GetById
        [HttpGet("{id}")]
        public IActionResult GetById([ModelBinder(Name = "id")] Author author) {
            if (author == null) {
                return NotFound();
            }

            return Ok(author);
        }
        #endregion

        #region snippet_Get
        [HttpGet("get/{author}")]
        public IActionResult Get(Author author) {
            if (author == null) {
                return NotFound();
            }

            return Ok(author);
        }
        #endregion
    }
}
