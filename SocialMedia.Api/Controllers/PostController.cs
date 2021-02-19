using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;

namespace SocialMedia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public PostController(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            return Ok(_mapper.Map<IEnumerable<PostDto>>(await _postRepository.GetPosts()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            return Ok(_mapper.Map<PostDto>(await _postRepository.GetPost(id)));
        }

        [HttpPost]
        public async Task<IActionResult> SetPost(PostDto postDto)
        {
            var post = _mapper.Map<Post>(postDto);
            await _postRepository.SetPost(post);
            return Ok(post);
        }
    }
}