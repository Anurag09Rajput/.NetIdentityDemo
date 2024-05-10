using Application.AngularIdentity.Concerns;
using Application.AngularIdentity.Contracts;
using Domain.AngularIdentity.Models.Models;
using Domain.AngularIdentity.Models.Request;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;

        public AuthSerivce(UserManager<User> useManager)
        {
            _userManager = useManager;
        }

        public async Task<Response> RegisterUser(UserForRegistrationDto model)
        {
            var userExists = await this._userManager.FindByNameAsync(model.Email!);
            if(userExists != null)
            {
                return new Response { IsSuccess = false, Message = "User already Exist!", StatusCode = HttpStatusCode.BadRequest };
            }

            User user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                /*PasswordHash = model.Password,*/
            };
            user.PasswordHash = this._userManager.PasswordHasher.HashPassword(user, model.Password!);

            var result  =  await this._userManager.CreateAsync(user,model.Password!);
            if (!result.Succeeded)
            {
                return new Response { IsSuccess = false, Message = "User creation failed! Please check user details and try again.", StatusCode = HttpStatusCode.BadRequest };
            }
          
            return new Response { IsSuccess = true, Message = "User added successfully.", StatusCode = HttpStatusCode.OK };
        }
    }
}
