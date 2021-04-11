﻿using Backend.DataAccess.Dogs;
using Backend.Models.DogBase;
using Backend.Models.DogBase.LostDog;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Backend.Tests.LostDogs
{
    [Collection("Database collection")]
    public class LostDogRepositoryTests
    {
        private readonly ILostDogRepository lostDogRepository;

        public LostDogRepositoryTests(DatabaseFixture databaseAuthFixture)
        {
            lostDogRepository = databaseAuthFixture.LostDogRepository;
        }

        [Fact]
        public async void AddingLostDogSuccessfulForValidDogInfo()
        {
            var saveDog = new LostDog()
            {
                Breed = "dogdog",
                Age = 5,
                Size = "Large, very large",
                Color = "Orange but a bit yellow and green dots",
                SpecialMark = "tattoo of you on the neck",
                Name = "Cat",
                Picture = new Picture()
                {
                    FileName = "photo",
                    FileType = "png",
                    Data = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
                HairLength = "Long",
                EarsType = "Short",
                TailLength = "None",
                Behaviors = new List<DogBehavior>() { new DogBehavior() { Behvaior = "Angry" } },
                Location = new Location() { City = "Biała", District = "Lol ther's none" },
                DateLost = new DateTime(2021, 3, 20),
                OwnerId = 1,
                Comments = new List<LostDogComment>()
            };
            var result = await lostDogRepository.AddLostDog(saveDog);
            Assert.True(result.Successful);
        }

        [Fact]
        public async void AddingLostDogFailsForMissingRequiredData()
        {
            var saveDogDog = new LostDog()
            {
                Breed = "dogdog",
                Age = 5,
                Size = "Large, very large",
                Color = "Orange but a bit yellow and green dots",
                SpecialMark = "tattoo of you on the neck",
                Name = "Cat",
                Picture = new Picture()
                {
                    FileName = "photo",
                    FileType = "png",
                    Data = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
                HairLength = "Long",
                TailLength = "None",
                Behaviors = new List<DogBehavior>() { new DogBehavior() { Behvaior = "Angry" } },
                Location = new Location() { City = "Biała", District = "Lol ther's none" },
                DateLost = new DateTime(2021, 3, 20),
                OwnerId = 1,
                Comments = new List<LostDogComment>()
            };
            var result = await lostDogRepository.AddLostDog(saveDogDog);
            Assert.False(result.Successful);
        }

        [InlineData(1)]
        [Theory]
        public async void GettingLostDogsForUserOneSuccessful(int userId)
        {
            var result = await lostDogRepository.GetUserLostDogs(userId);
            Assert.True(result.Successful);
        }

        [InlineData(1)]
        [Theory]
        public async void GettingLostDogDetailsForDogOneSuccessful(int dogId)
        {
            var result = await lostDogRepository.GetLostDogDetails(dogId);
            Assert.True(result.Successful);
        }

        [Fact]
        public async void DeletingLostDogClearsAllData()
        {
            var saveDog = new LostDog()
            {
                Breed = "dogdog",
                Age = 5,
                Size = "Large, very large",
                Color = "Orange but a bit yellow and green dots",
                SpecialMark = "tattoo of you on the neck",
                Name = "Cat",
                Picture = new Picture()
                {
                    FileName = "photo",
                    FileType = "png",
                    Data = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
                HairLength = "Long",
                EarsType = "Short",
                TailLength = "None",
                Behaviors = new List<DogBehavior>() { new DogBehavior() { Behvaior = "Angry" } },
                Location = new Location() { City = "Biała", District = "Lol ther's none" },
                DateLost = new DateTime(2021, 3, 20),
                OwnerId = 1,
                Comments = new List<LostDogComment>()
            };

            var dog = await lostDogRepository.AddLostDog(saveDog);
            Assert.True(dog.Successful);

            var result = await lostDogRepository.DeleteLostDog(dog.Data.Id);
            Assert.True(result.Successful);

            dog = await lostDogRepository.GetLostDogDetails(dog.Data.Id);
            Assert.False(dog.Successful);
        }

        [Fact]
        public async void MarkingLostDogAsFoundSuccessfulForExistingDog()
        {
            var savedDogs = await lostDogRepository.GetLostDogs();

            Assert.True(savedDogs.Successful);
            Assert.NotEmpty(savedDogs.Data);
            var response = await lostDogRepository.MarkDogAsFound(savedDogs.Data.First().Id);
            Assert.True(response.Successful);
        }

        [Fact]
        public async void MarkingLostDogAsFoundFailsForNonExistingDog()
        {
            var response = await lostDogRepository.MarkDogAsFound(-1);
            Assert.False(response.Successful);
        }

        [Fact]
        public async void MarkingLostDogAsFoundFailsForDogMarkedAsFound()
        {
            var saveDog = new LostDog()
            {
                Breed = "dogdog",
                Age = 5,
                Size = "Large, very large",
                Color = "Orange but a bit yellow and green dots",
                SpecialMark = "tattoo of you on the neck",
                Name = "Cat",
                Picture = new Picture()
                {
                    FileName = "photo",
                    FileType = "png",
                    Data = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
                HairLength = "Long",
                EarsType = "Short",
                TailLength = "None",
                Behaviors = new List<DogBehavior>() { new DogBehavior() { Behvaior = "Angry" } },
                Location = new Location() { City = "Biała", District = "Lol ther's none" },
                DateLost = new DateTime(2021, 3, 20),
                OwnerId = 1,
                Comments = new List<LostDogComment>()
            };
            var result = await lostDogRepository.AddLostDog(saveDog);
            Assert.NotNull(result);
            var response = await lostDogRepository.MarkDogAsFound(saveDog.Id);
            Assert.True(response.Successful);
            response = await lostDogRepository.MarkDogAsFound(saveDog.Id);
            Assert.False(response.Successful);
        }
    
        [Fact]
        public async void UpdatingLostDogSuccessfulForExistingDog()
        {
            var lostDog = new LostDog()
            {
                Breed = "dogdog",
                Age = 5,
                Size = "Large, very large",
                Color = "Orange but a bit yellow and green dots",
                SpecialMark = "tattoo of you on the neck",
                Name = "Cat",
                Picture = new Picture()
                {
                    FileName = "photo",
                    FileType = "png",
                    Data = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
                HairLength = "Long",
                EarsType = "Short",
                TailLength = "None",
                Behaviors = new List<DogBehavior>() { new DogBehavior() { Behvaior = "Angry" } },
                Location = new Location() { City = "Biała", District = "Lol ther's none" },
                DateLost = new DateTime(2021, 3, 20),
                OwnerId = 1,
                Comments = new List<LostDogComment>()
            };
            var result = await lostDogRepository.AddLostDog(lostDog);
            Assert.True(result.Successful);
            lostDog = result.Data;
            lostDog.Age = 6;
            Assert.True((await lostDogRepository.UpdateLostDog(lostDog)).Successful);
        }

        [Fact]
        public async void UpdatingLostDogFailsForNonExistingDog()
        {
            var lostDog = new LostDog();
            lostDog.Id = -1;
            Assert.False((await lostDogRepository.UpdateLostDog(lostDog)).Successful);
        }

        [Fact]
        public async void UpdatingLostDogSuccessfulForInvalidData()
        {
            var lostDog = new LostDog()
            {
                Breed = "dogdog",
                Age = 5,
                Size = "Large, very large",
                Color = "Orange but a bit yellow and green dots",
                SpecialMark = "tattoo of you on the neck",
                Name = "Cat",
                Picture = new Picture()
                {
                    FileName = "photo",
                    FileType = "png",
                    Data = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
                HairLength = "Long",
                EarsType = "Short",
                TailLength = "None",
                Behaviors = new List<DogBehavior>() { new DogBehavior() { Behvaior = "Angry" } },
                Location = new Location() { City = "Biała", District = "Lol ther's none" },
                DateLost = new DateTime(2021, 3, 20),
                OwnerId = 1,
                Comments = new List<LostDogComment>()
            };
            var result = await lostDogRepository.AddLostDog(lostDog);
            Assert.True(result.Successful);
            lostDog = result.Data;
            lostDog.Breed = null;
            Assert.False((await lostDogRepository.UpdateLostDog(lostDog)).Successful);
        }
    }
}
