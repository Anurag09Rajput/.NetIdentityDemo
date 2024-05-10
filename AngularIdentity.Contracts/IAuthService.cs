using Application.AngularIdentity.Concerns;
using Domain.AngularIdentity.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AngularIdentity.Contracts
{
    public interface IAuthService
    {
        public Task<Response> RegisterUser(UserForRegistrationDto model);
    }
}
