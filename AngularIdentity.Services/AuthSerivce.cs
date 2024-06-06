using AngularIdentity.Models.Request;
using Application.AngularIdentity.Concerns;
using Application.AngularIdentity.Contracts;
using Domain.AngularIdentity.Models.Models;
using Domain.AngularIdentity.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.AngularIdentity.Services
{
    public class AuthSerivce: IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly JwtHandler _jwtHandler;

        public AuthSerivce(UserManager<User> useManager , IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = useManager;
            _jwtHandler = new JwtHandler(configuration);
        }

        public async Task<Response> RegisterUser(UserForRegistrationDto model)
        {
            var userExists = await this._userManager.FindByEmailAsync(model.Email!);
            if(userExists != null)
            {
                return new Response { IsSuccess = false, Message = "User already Exist!", StatusCode = HttpStatusCode.BadRequest };
            }

            User user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.FirstName + model.LastName,
            };

            
            var result  =  await this._userManager.CreateAsync(user,model.Password!);
            if (!result.Succeeded)
            {
                return new Response { IsSuccess = false, Message = "User creation failed! Please check user details and try again.", StatusCode = HttpStatusCode.BadRequest };
            }
          
            return new Response { IsSuccess = true, Message = "User added successfully.", StatusCode = HttpStatusCode.OK };
        }

        public async Task<Response> LoginUser(UserForAuthenticationDto model)
        {
            var user =  await _userManager.FindByEmailAsync(model.Email!);

            if( user == null)
            {
                return new Response { IsSuccess = false, Message = "User not found!" };
            }

            if(await _userManager.CheckPasswordAsync(user, model.Password!))
            {
                JwtHandler jwt = new JwtHandler(_configuration);
                var claims = jwt.GetClaims(model);
                string token = jwt.GenerateTokenOptions(claims);

                return new Response<string> { IsSuccess = true, Data = token.ToString(), Message = "User authenticated successfully.", StatusCode = HttpStatusCode.OK };
            }

            return new Response { IsSuccess = false, Message = "Something went wrong!" };
        }

        public async Task<Response> ExternalLogin(ExternalAuthDto externalAuth)
        {
            var payload = await this._jwtHandler.VerifyGoogleToken(externalAuth);
            if(payload == null)
                return new Response(){IsSuccess= false, Message="Invalid External Authentication"};

            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new User { Email = payload.Email, UserName = payload.Email };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }

            }

            if(user == null)
            {
                return new Response() { IsSuccess = false, Message = "Invalid External Authentication" };
            }

            var claim =  _jwtHandler.GetClaims(user);
            var token = _jwtHandler.GenerateTokenOptions(claim);

            return new Response<string> { IsSuccess = true, Data = token.ToString(), Message = "User authenticated successfully.", StatusCode = HttpStatusCode.OK };
        }
    }
}
