using System;
using AspNetCore.Identity.MongoDbCore.Models;

namespace IdentityServer.Models
{
    public class ApplicationRole : MongoIdentityRole<Guid>
{
	public ApplicationRole() : base()
	{
	}

	public ApplicationRole(string roleName) : base(roleName)
	{
	}
}
}