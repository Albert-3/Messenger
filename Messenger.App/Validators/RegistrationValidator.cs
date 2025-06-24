using Messenger.App.DTOs;
using FluentValidation;

namespace Messenger.App.Validators
{
    public class RegistrationValidator : AbstractValidator<RegistrationDTO>
    {
        public RegistrationValidator()
        {
            RuleFor(user => user.Password).NotEmpty().NotNull().Equal(user => user.RepeatPassword).WithMessage("Invalid password");
            RuleFor(user => user.UserName).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$").WithMessage("Invalid userName");
            RuleFor(user => user.PhoneNumber).NotNull().NotEmpty().Matches(@"^\+?\d+$").WithMessage("Invalid phone number");
        }
    }
}
