﻿using AutoMapper;
using Backend.DTOs.Authentication;
using Backend.Models.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.AuthenticationService
{
    public class AccountService : IAccountService

    {
        private readonly UserManager<Account> userManager;
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ILogger<AccountService> logger;

        public AccountService(UserManager<Account> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration, IMapper mapper, ILogger<AccountService> logger)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.mapper = mapper;
            this.logger = logger;

            ConfigureRoles().Wait();
        }

        private async Task<bool> ConfigureRoles()
        {
            try
            {
                if (!await roleManager.RoleExistsAsync(AccountRoles.User))
                    await roleManager.CreateAsync(new IdentityRole<int>(AccountRoles.User));
                if (!await roleManager.RoleExistsAsync(AccountRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole<int>(AccountRoles.Admin));
                if (!await roleManager.RoleExistsAsync(AccountRoles.Shelter))
                    await roleManager.CreateAsync(new IdentityRole<int>(AccountRoles.Shelter));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public async Task<ServiceResponse<GetAccountDto>> AddAccount(AddAccountDto _account)
        {
            var serviceResponse = new ServiceResponse<GetAccountDto>();
            
            var userExists = await userManager.FindByNameAsync(_account.UserName);
            if (userExists != null)
            {
                serviceResponse.Message = "User already exists!";
                serviceResponse.Successful = false;
                serviceResponse.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                Account account = mapper.Map<Account>(_account);
                account.SecurityStamp = Guid.NewGuid().ToString();
                var result = await userManager.CreateAsync(account, _account.Password);
                if (!result.Succeeded)
                {
                    serviceResponse.Message = $"User creation failed! {string.Join(", ", result.Errors)}";
                    serviceResponse.Successful = false;
                    serviceResponse.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    result = await userManager.AddToRoleAsync(account, AccountRoles.User);
                    var savedUser = await userManager.FindByNameAsync(_account.UserName);
                    if (!result.Succeeded)
                    {
                        serviceResponse.Message = $"Failed to add user to the role! {string.Join(", ", result.Errors)}";
                        serviceResponse.Successful = false;
                        serviceResponse.StatusCode = StatusCodes.Status400BadRequest;
                        await userManager.DeleteAsync(savedUser);
                    }
                    else
                    {
                        serviceResponse.Message = $"User sucessfully created!";
                        serviceResponse.StatusCode = StatusCodes.Status201Created;
                        serviceResponse.Data = mapper.Map<GetAccountDto>(savedUser);
                    }
                }
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAccountDto>> GetAccountById(int id)
        {
            var serviceResponse = new ServiceResponse<GetAccountDto>();
            var savedUser = await userManager.FindByIdAsync(id.ToString());
            if (savedUser == null)
            {
                serviceResponse.StatusCode = StatusCodes.Status404NotFound;
                serviceResponse.Successful = false;
                serviceResponse.Message = $"Failed to fetch User with id: {id}";
            }
            else
            {
                serviceResponse.Data = mapper.Map<GetAccountDto>(savedUser);
                serviceResponse.Message = "User found";
                serviceResponse.StatusCode = StatusCodes.Status200OK;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<IList<GetAccountDto>>> GetAllAccountsForRole(string role)
        {
            var serviceResponse = new ServiceResponse<IList<GetAccountDto>>();
            var savedUsers =  await userManager.GetUsersInRoleAsync(role);
            if (savedUsers == null)
            {
                serviceResponse.StatusCode = StatusCodes.Status500InternalServerError;
                serviceResponse.Successful = false;
                serviceResponse.Message = "Failed to fetch Users";
            } 
            else
            {
                serviceResponse.Data = mapper.Map<IList<GetAccountDto>>(savedUsers);
                serviceResponse.Message = $"{savedUsers.Count} User found";
                serviceResponse.StatusCode = StatusCodes.Status200OK;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<AuthenticationResult>> Authenticate(LoginDto loginDto)
        {
            var serviceResponse = new ServiceResponse<AuthenticationResult>();
            var user = await userManager.FindByNameAsync(loginDto.UserName);
            if (user != null)
            {
                if (await userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    var userRole = (await userManager.GetRolesAsync(user)).First();
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Role, userRole)
                    };

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: configuration["JWT:ValidIssuer"],
                        audience: configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );
                    serviceResponse.Data = new AuthenticationResult()
                    {
                        UserType = userRole,
                        Id = user.Id,
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                    };
                }
                else
                {
                    serviceResponse.StatusCode = StatusCodes.Status400BadRequest;
                    serviceResponse.Message = "Invalid user password";
                    serviceResponse.Successful = false;
                }
            }
            else
            {
                serviceResponse.StatusCode = StatusCodes.Status404NotFound;
                serviceResponse.Message = "User not defined in the system";
                serviceResponse.Successful = false;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAccountDto>> UpdateAccount(UpdateAccountDto accountDto, int userId)
        {
            var savedAccount = await userManager.FindByIdAsync(userId.ToString());
            var serviceResponse = new ServiceResponse<GetAccountDto>();
            if (savedAccount != null)
            {
                savedAccount.UserName = accountDto.UserName;
                savedAccount.Email = accountDto.Email;
                savedAccount.PhoneNumber = accountDto.PhoneNumber;
                var result = await userManager.UpdateAsync(savedAccount);
                serviceResponse = await GetAccountById(userId);
                if (result.Succeeded && serviceResponse.Successful)
                {
                    serviceResponse.Message = $"User with id {userId} was updated";
                    serviceResponse.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    serviceResponse.Message = $"Failed to update user with id {userId}: {string.Join(", ", result.Errors)}";
                    serviceResponse.Successful = false;
                    serviceResponse.StatusCode = StatusCodes.Status500InternalServerError;
                }
            }
            else
            {
                serviceResponse.Message = $"Failed to update user with id {userId}: Account not found";
                serviceResponse.Successful = false;
                serviceResponse.StatusCode = StatusCodes.Status404NotFound;
            }
            return serviceResponse;
        }
    }
}
