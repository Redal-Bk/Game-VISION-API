using FluentValidation;
using Game_Vision.Application.Command.Auth;
using Game_Vision.Models;
using Microsoft.EntityFrameworkCore;


namespace Game_Vision.Application.Validator
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator(GameVisionDbContext context)
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("نام کاربری الزامی است")
                .Length(3, 50).WithMessage("نام کاربری باید بین ۳ تا ۵۰ کاراکتر باشد")
                .MustAsync(async (username, ct) => !await context.Users.AnyAsync(u => u.Username == username, ct))
                .WithMessage("این نام کاربری قبلاً استفاده شده");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ایمیل الزامی است")
                .EmailAddress().WithMessage("فرمت ایمیل صحیح نیست")
                .MustAsync(async (email, ct) => !await context.Users.AnyAsync(u => u.Email == email, ct))
                .WithMessage("این ایمیل قبلاً ثبت شده");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("رمز عبور الزامی است")
                .MinimumLength(6).WithMessage("رمز عبور باید حداقل ۶ کاراکتر باشد");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("رمز عبور و تکرار آن مطابقت ندارند");
        }
    }
}
