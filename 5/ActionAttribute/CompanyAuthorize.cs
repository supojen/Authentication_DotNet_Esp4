using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _5.ActionAttribute
{
    public class CompanyAuthorize : AuthorizeAttribute
    {
        public CompanyAuthorize(string department, int level)
        {
            Policy = $"{department}.{level}";
        }
    }
}
