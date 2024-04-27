using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetworkingPlatform.Data;
using NetworkingPlatform.Models;

namespace NetworkingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetUsers")]
        public List<Users> GetUsers()
        {

            return _context.Users.ToList();
        }

        [HttpGet]
        [Route("GetUser")]
        public Users GetUser(int id)
        {

            return _context.Users.Where(x=>x.ID==id).FirstOrDefault();
        }

        [HttpPost]
        [Route("AddUser")]
        public string AddUser(Users User) {
            _context.Users.Add(User);
            _context.SaveChanges();
            return "User Added Sucessfully!";
        }


    }
}
