using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Api.Responses;
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
            return Ok(new ApiResponse<IEnumerable<PostDto>>(_mapper.Map<IEnumerable<PostDto>>(await _postRepository.GetPosts())));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            return Ok(new ApiResponse<PostDto>(_mapper.Map<PostDto>(await _postRepository.GetPost(id))));
        }

        [HttpPost]
        public async Task<IActionResult> Post(PostDto postDto)
        {
            var post = _mapper.Map<Post>(postDto);
            await _postRepository.InsertPost(post);
            postDto = _mapper.Map<PostDto>(post);
            var response = new ApiResponse<PostDto>(postDto);

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, PostDto postDto)
        {
            var post = _mapper.Map<Post>(postDto);
            post.PostId = id;
            var result = await _postRepository.UpdatePost(post);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _postRepository.DeletePost(id);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }
    }
}