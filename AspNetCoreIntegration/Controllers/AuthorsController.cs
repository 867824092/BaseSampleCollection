using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AspNetCoreIntegration.Data;

namespace AspNetCoreIntegration.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : Controller {
        private readonly AuthorContext _context;

        public AuthorsController(AuthorContext context) {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id) {
            var author = _context.Authors.Find(id);

            if (author == null) {
                return NotFound();
            }

            return Ok(author);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Author author) {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // [HttpPost]
        // public IActionResult Post([FromBody]TestModel testModel) {
        //     return Ok();
        // }
    }
    public class TestModel {
        [Required(ErrorMessage ="名称不能为空")]
        public string Name { get; set; }
    }
}
