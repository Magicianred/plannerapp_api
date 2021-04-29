﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PlannerApp.Models;
using PlannerApp.Server.Exceptions;
using PlannerApp.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannerApp.Server.Controllers.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IConfiguration _configuration;
        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        // /api/auth/register
        [HttpPost("Register")]
        [ProducesResponseType(200, Type = typeof(OperationResponse<UserManagerResponse>))]
        [ProducesResponseType(400, Type = typeof(ApiErrorResponse))]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Some properties are not valid", ModelState.Select(m => $"{m.Key} - {m.Value}").ToArray());

            var result = await _userService.RegisterUserAsync(model);

            return Ok(new OperationResponse<UserManagerResponse>()
            {
                IsSuccess = true,
                Message = "User registered successfully",
                OperationDate = DateTime.UtcNow,
                Record = result
            });
        }

        // /api/auth/login
        [HttpPost("Login")]
        [ProducesResponseType(200, Type = typeof(OperationResponse<UserManagerResponse>))]
        [ProducesResponseType(400, Type = typeof(ApiErrorResponse))]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Some properties are not valid", ModelState.Select(m => $"{m.Key} - {m.Value}").ToArray());

            var result = await _userService.LoginUserAsync(model);

            return Ok(new OperationResponse<UserManagerResponse>()
            {
                IsSuccess = true,
                Message = "User registered successfully",
                OperationDate = DateTime.UtcNow,
                Record = result
            });
        }

    }
}
