using Microsoft.AspNetCore.Mvc;
using ProductService2.Abstract_;
using ProductService2.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductService2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepo<User> _userRepo;

        public UserController(IRepo<User> userRepo)
        {
            _userRepo = userRepo;
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            return await _userRepo.GetAllAsync();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<User> Get(Guid id)
        {
            return await _userRepo.GetAsync(id);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<User> Post([FromBody] User entity)
        {
            entity.Id = Guid.NewGuid();
            await _userRepo.AddAsync(entity);
            return entity;
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] User entity)
        {
            var user = await _userRepo.GetAsync(id);
            if (user == null)
                return BadRequest("user not exists");

            await _userRepo.UpdateAsync(user);
            return Ok();
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userRepo.GetAsync(id);
            if (user == null)
                return BadRequest("user not exists");

            await _userRepo.DeleteAsync(id);
            return Ok();
        }
    }
}
