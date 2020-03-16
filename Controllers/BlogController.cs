using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace eleven.Controllers
{
    [Route("api/[controller]")]

    public class BlogController : ControllerBase
    {
        public BlogController(appDb db)
        {
            Db = db;
        }

        // GET api/blog/user
 

        [HttpGet("user")]
        public async Task<IActionResult> GetUserInfo()
        {
            await Db.Connection.OpenAsync();
            var query = new employee(Db);
            var result = await query.getUserInformation();
            return new OkObjectResult(result);
        }
        
        
        // GET api/blog/user/5
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new employee(Db);
            var result = await query.getUserInformationById(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }
        [HttpGet("user/GetUser/{name}")]
        public async Task<IActionResult> GetUser(string name)
        {
            await Db.Connection.OpenAsync();
            var query = new employee(Db);
            var result = await query.getUserInformationByName(name);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

      
        [HttpPut("user/{id}/{firstName}/{addressLine1}")]
        public async Task<IActionResult> PutOne(int id, string firstName,string  addressLine1)
        {
            await Db.Connection.OpenAsync();
            var query = new employee(Db);
            var result = await query.UpdateUserInformationById(id,firstName,addressLine1);
           
            if (result is null)
                return new NotFoundResult();
           
          
            return new OkObjectResult(result);
        }


        public appDb Db { get; }

        public class BlogPost
        {
        }
    }

}
