using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.API.Resources
{
    public class TokenResource
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
