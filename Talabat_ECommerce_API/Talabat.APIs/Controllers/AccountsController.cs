﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	public class AccountsController : APIBaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountsController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager
			                      ,ITokenService tokenService
			                      ,IMapper mapper)
        {
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
			_mapper = mapper;
		}

        //Register
        [HttpPost("Register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			if (CheckEmailExists(model.Email).Result.Value)
				return BadRequest(new ApiResponse(400,"This Email Is Already Exist"));


			var User = new AppUser()
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				UserName = model.Email.Split('@')[0],
				PhoneNumber = model.PhoneNumber,

			};

			var Result = await _userManager.CreateAsync(User, model.Password);
			if (!Result.Succeeded) return BadRequest(new ApiResponse(400));

			var ReturnedUser = new UserDto()
			{
				DisplayName = User.DisplayName,
				Email= User.Email,
				Token =await _tokenService.CreateTokenAsync(User,_userManager)
			};
			return Ok(ReturnedUser);
		}


		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		{
			var User = await _userManager.FindByEmailAsync(model.Email);
			if (User is null) return Unauthorized(new ApiResponse(401));

			var Result= await _signInManager.CheckPasswordSignInAsync(User, model.Password,false);
			if (!Result.Succeeded) return Unauthorized(new ApiResponse(401));

			return Ok(new UserDto()
			{
				DisplayName = User.DisplayName,
				Email = User.Email,
				Token = await _tokenService.CreateTokenAsync(User, _userManager)
			});
		}

		//Get Current User
		[Authorize]
		[HttpGet("GetCurrentUser")]
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var Email = User.FindFirstValue(ClaimTypes.Email);
			var user = await _userManager.FindByEmailAsync(Email);

			var ReturnedUser = new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)

			};
			return Ok(ReturnedUser);
		}

		[Authorize]
		[HttpGet("Address")]
		public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
		{
			
			var user = await _userManager.FindUserWithAddressAsync(User);
			var MappedAddress=_mapper.Map<Address,AddressDto>(user.Address);

			return Ok(MappedAddress);
		}

		[Authorize]
		[HttpPut("Address")]
		public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto UpdatedAddress)
		{
			var user = await _userManager.FindUserWithAddressAsync(User);
			if (user is null) return Unauthorized(new ApiResponse(401));
			var address = _mapper.Map<AddressDto, Address>(UpdatedAddress);
			address.Id = user.Address.Id;
			user.Address = address;
			var Result = await _userManager.UpdateAsync(user);
			if (!Result.Succeeded) return BadRequest(new ApiResponse(400));
			return Ok(UpdatedAddress);
		}

		[HttpGet("emailExists")]

		public async Task<ActionResult<bool>> CheckEmailExists(string email)
		{
			return await _userManager.FindByEmailAsync(email) is not null;
		}

	}
}
