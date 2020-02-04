using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    public class ApplicationRole
    {

        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public string ConcurrencyStamp { get; set; }

        public List<ApplicationClaim> Claims { get; set; } = new List<ApplicationClaim>();
        



        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName)
        {
            this.Id = roleName.Trim().Replace(' ', '.').ToLower();
            this.Name = roleName;
        }
    }
}