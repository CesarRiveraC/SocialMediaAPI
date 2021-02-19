using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;

        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            return Ok(await _postRepository.GetPosts());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            return Ok(await _postRepository.GetPost(id));
        }

        [HttpPost]
        public async Task<IActionResult> SetPost(Post post)
        {
            await _postRepository.SetPost(post);
            return Ok(post);
        }
    }
}