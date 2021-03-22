﻿using AutoMapper;
using Backend.DTOs.Dogs;
using Backend.Models.DogBase.LostDog;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DataAccess.Dogs
{
    public class LostDogDataRepository : ILostDogRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<LostDogDataRepository> _logger;

        public LostDogDataRepository(ApplicationDbContext dbContext, ILogger<LostDogDataRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<LostDog> AddLostDog(LostDog lostDog)
        {
            try
            {
                var returningDog = await _dbContext.LostDogs.AddAsync(lostDog);
                await _dbContext.SaveChangesAsync();
                return returningDog.Entity;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteLostDog(int dogId)
        {
            try
            {
                var lostDog = _dbContext.LostDogs.Find(dogId);
                if (lostDog == null) throw new Exception();
                _dbContext.LostDogs.Remove(lostDog);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<LostDog> GetLostDogDetails(int dogId)
        {
            return _dbContext.LostDogs.Find(dogId);
        }

        public async Task<List<LostDog>> GetLostDogs()
        {
            try
            {
                return _dbContext.LostDogs.OrderBy(ld => ld.Id).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<LostDog>> GetUserLostDogs(int ownerId)
        {
            try
            {
                return _dbContext.LostDogs.Where(ld => ld.OwnerId == ownerId).OrderBy(ld => ld.Id).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<LostDogComment> AddLostDogComment(LostDogComment comment)
        {
            try
            {
                var lostDog = _dbContext.LostDogs.Find(comment.DogId);
                if (lostDog == null) throw new Exception();
                lostDog.Comments.Add(comment);
                _dbContext.LostDogs.Update(lostDog);
                _dbContext.SaveChanges();
                return comment;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<LostDogComment>> GetLostDogComments(int dogId)
        {
            try
            {
                return _dbContext.LostDogs.Find(dogId).Comments;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<LostDogComment> EditLostDogComment(LostDogComment comment)
        {
            try
            {
                var lostDog = _dbContext.LostDogs.Find(comment.DogId);
                if (lostDog == null) throw new Exception();
                var oldComment = lostDog.Comments.Find(c => c.Id == comment.Id);
                if (oldComment == null) throw new Exception();
                oldComment = comment;
                _dbContext.LostDogs.Update(lostDog);
                _dbContext.SaveChanges();
                return comment;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
