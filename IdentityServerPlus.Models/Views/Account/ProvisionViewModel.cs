namespace IdentityServer.Models.Views
{
    public class ProvisionViewModel
    {
        /// <summary>Is this Provision for a new user?</summary>
        public bool RegisterUser { get; set; }


        /// <summary>Is this Provision for an existing user?</summary>
        public bool LinkAccount => !RegisterUser;


        public string Provider { get; set; }



        public string FirstName { get; set; }
        public string FamilyName { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }



    }
}