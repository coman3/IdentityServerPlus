using IdentityServer.Models.Views;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.ThemeProvider.Modern.Data
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() { return new IdentityError { Code = nameof(DefaultError), Description = $"An unknown failure has occurred." }; }
        public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Optimistic concurrency failure, object has been modified" }; }
        public override IdentityError PasswordMismatch() { return new IdentityError { Code = nameof(RegisterViewModel.Password), Description = "Incorrect password" }; }
        public override IdentityError InvalidToken() { return new IdentityError { Code = nameof(InvalidToken), Description = "Invalid token" }; }
        public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = nameof(RegisterViewModel.Email), Description = "Already exists" }; }
        public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = nameof(RegisterViewModel.Username), Description = $"No special characters" }; }
        public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = nameof(RegisterViewModel.Email), Description = $"Invalid email" }; }
        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(RegisterViewModel.Username), Description = $"Already exists" }; }
        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(RegisterViewModel.Email), Description = $"Already exists" }; }
        public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = nameof(InvalidRoleName), Description = $"Role name '{role}' is invalid." }; }
        public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"Role name '{role}' is already taken." }; }
        public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "Already has password" }; }
        public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = nameof(RegisterViewModel.Username), Description = "User locked out" }; }
        public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"User already in role '{role}'." }; }
        public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = nameof(UserNotInRole), Description = $"User is not in role '{role}'." }; }
        public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = nameof(PasswordTooShort), Description = $"Must be between 6 and 128 characters" }; }
        public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = nameof(RegisterViewModel.Password), Description = "Mising special character" }; }
        public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = nameof(RegisterViewModel.Password), Description = "Missing a digit" }; }
        public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = nameof(RegisterViewModel.Password), Description = "Missing lowercase letter" }; }
        public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = nameof(RegisterViewModel.Password), Description = "Missing uppercase letter" }; }
    }
}
