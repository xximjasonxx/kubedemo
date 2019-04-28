
using Microsoft.AspNetCore.Mvc;

namespace StockPriceApi.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult RunTest()
        {
            return Ok("This is a Test - and it worked");
        }
    }
}