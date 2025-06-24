using FluentValidation;
using Messenger.App.DTOs;

namespace Messenger.App.Validators
{
    public class LoginValidator : AbstractValidator<LoginDTO>
    {
        public LoginValidator()
        {
            RuleFor(user => user.Password).NotEmpty().NotNull().WithMessage("Password is required.");
            RuleFor(user => user.UserName).NotEmpty().NotNull().WithMessage("User name is required.");
        }
    }
}
