using api_bui_xuan_thang.Models;
using api_bui_xuan_thang.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace api_bui_xuan_thang.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly ApplicationDbContext _dbContext;

        public PostController(IPostRepository postRepository, ApplicationDbContext dbContext)
        {
            _postRepository = postRepository;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postRepository.GetAllAsync();
            var result = posts.Select(post => new
            {
                post.Id,
                post.Title,
                post.Description,
                post.Image,
                post.DateCreate,
                User = post.User != null ? new
                {
                    post.User.Id,
                    post.User.UserName,
                    post.User.Avatar
                } : null
            });

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var posts = await _postRepository.GetByUserIdAsync(userId);

            var result = posts.Select(post => new
            {
                post.Id,
                post.Title,
                post.Description,
                post.Image,
                post.DateCreate,
                User = post.User != null ? new
                {
                    post.User.Id,
                    post.User.UserName,
                } : null
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null) return NotFound();

            // Nếu post chưa có thông tin User, lấy thêm từ ApplicationDbContext (trực tiếp)
            if (post.User == null && !string.IsNullOrEmpty(post.UserId))
            {
                post.User = await _dbContext.Users.FindAsync(post.UserId);
            }

            var result = new
            {
                post.Id,
                post.Title,
                post.Description,
                post.Image,
                post.DateCreate,
                User = post.User != null ? new
                {
                    post.User.Id,
                    post.User.UserName,
                } : null
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Post post)
        {
            // Lấy UserId từ Claims trong JWT
            // var userId = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(post.UserId))
            {
                return Unauthorized("UserId not found in token.");
            }

            try
            {
                var newPost = await _postRepository.CreateAsync(post, post.UserId);
                return CreatedAtAction(nameof(GetById), new { id = newPost.Id }, newPost);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Post post)
        {
            if (string.IsNullOrEmpty(post.UserId))
            {
                return BadRequest("UserId is required.");
            }

            try
            {
                await _postRepository.UpdateAsync(post, post.UserId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string userId)
        {
            try
            {
                await _postRepository.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
