using LocalizationDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Xml.Linq;

namespace LocalizationDemo.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IHtmlLocalizer<HomeController> _localizer;

        public HomeController(ILogger<HomeController> logger,
            IHtmlLocalizer<HomeController> localizer) {
            _logger = logger;
            _localizer = localizer;
        }

        public IActionResult Entity() {
            var model = new
                TableStructure()
                { NameSpace = "RazorViewTemplateEngine.Tests"};
            model.Columns.Add(new ColumnDescription()
            {
            ColumnName = "Id",
            ColumnType = "int",
            Comment = "主键",
            IsCanNull = false
            }); model.Columns.Add(new ColumnDescription()
            {
            ColumnName = "Name",
            ColumnType = "string",
            Comment = "姓名",
            IsCanNull = false
            });
            return View(model);
        }
        public IActionResult Index() {
            ViewData["Message"] = _localizer["<b>Hello</b><i> {0}</i>", "Admin"];
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}