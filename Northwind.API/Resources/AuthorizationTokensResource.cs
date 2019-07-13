using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.API.Resources
{
    public class AuthorizationTokensResource
    {
        public TokenResource AccessToken { get; set; }
        public TokenResource RefreshToken { get; set; }
    }
}
