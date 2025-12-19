using FluentValidation;
using Game_Vision.Application.Command.Auth;

namespace Game_Vision.Application.Validator
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.UsernameOrEmail).NotEmpty().WithMessage("نام کاربری یا ایمیل الزامی است");
            RuleFor(x => x.Password).NotEmpty().WithMessage("رمز عبور الزامی است");
        }
    }
}
