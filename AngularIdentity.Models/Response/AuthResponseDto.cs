using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AngularIdentity.Models.Response
{
    public class AuthResponseDto
    {
        public bool IsAuthSuccessful { get; set; }
        public IEnumerable<string>? ErrorMessage { get; set; }
        public string? Token { get; set; }
    }
}
