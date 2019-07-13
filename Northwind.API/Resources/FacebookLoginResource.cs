using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.API.Resources
{
    public class FacebookLoginResource
    {
        [Required]
        [StringLength(255)]
        public string facebookToken { get; set; }
    }
}
