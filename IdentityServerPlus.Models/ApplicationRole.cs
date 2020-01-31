using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    public class ApplicationRole : IdentityRole<Guid>
{
	public ApplicationRole() : base()
	{
	}

	public ApplicationRole(string roleName) : base(roleName)
	{
	}
}
}