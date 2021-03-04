﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;

namespace SocialMedia.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Post> GetPost(int id)
        {
            return await _unitOfWork.PostRepository.GetById(id);
        }

        public PagedList<Post> GetPosts(PostQueryFilter filters)
        {
            var posts = _unitOfWork.PostRepository.GetAll();
            if (filters.UserId != null)
            {
                posts = posts.Where(x => x.UserId == filters.UserId);
            }

            if (filters.Date != null)
            {
                posts = posts.Where(x => x.Date.ToShortDateString() == filters.Date?.ToShortDateString());
            }

            if (filters.Description != null)
            {
                posts = posts.Where(x => x.Description.ToLower().Contains(filters.Description.ToLower()));
            }

            var pagedPosts = PagedList<Post>.CreatePagedList(posts, filters.PageNumber, filters.PageSize);

            return pagedPosts;
        }

        public async Task InsertPost(Post post)
        {
            //to be able to insert a post first, we need to check if the user is registered in the system.
            var user = await _unitOfWork.UserRepository.GetById(post.UserId);
            if (user == null)
            {
                throw new BusinessException("User don't exist");
            }

            //Second, if the user have less than 10 posts in total,  is able to insert just 1 post per week.
            var userPost = await _unitOfWork.PostRepository.GetPostsByUser(post.UserId);
            if (userPost.Count() < 10)
            {
                var lastPost = userPost.OrderByDescending(x => x.Date).FirstOrDefault();
                var totalDays = (DateTime.Now - lastPost.Date).TotalDays;
                var daysLeftToPost = 7 - totalDays;
                if (totalDays < 7)
                {
                    throw new BusinessException("You're not able to post yet, try again in " + (int) daysLeftToPost +
                                                " days.");
                }
            }

            //last, verify if user is posting the word "sexo"
            if (post.Description.Contains("sexo"))
            {
                throw new BusinessException("Content not allowed");
            }

            await _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdatePost(Post post)
        {
            _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePost(int id)
        {
            await _unitOfWork.PostRepository.Delete(id);
            return true;
        }
    }
}