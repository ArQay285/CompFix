using Microsoft.AspNetCore.Identity;

namespace CompFix.Services
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError {
                Code = nameof(PasswordTooShort),
                Description = $"Пароль занадто простий! Мінімальна довжина пароля — {length} символів."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError {
                Code = nameof(PasswordRequiresDigit),
                Description = "Пароль має містити хоча б одну цифру ('0'-'9')."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError {
                Code = nameof(PasswordRequiresLower),
                Description = "Пароль має містити хоча б одну малу літеру ('a'-'z')."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError {
                Code = nameof(PasswordRequiresUpper),
                Description = "Пароль має містити хоча б одну велику літеру ('A'-'Z')."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Пароль має містити хоча б один спеціальний символ (!@#$%^&*_-)."
            };
        }
    }
}