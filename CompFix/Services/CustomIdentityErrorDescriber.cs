using Microsoft.AspNetCore.Identity;

namespace CompFix.Services
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        // Перевизначення повідомлень про помилки для локалізації на українську мову
        // Пароль
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

        // Пошта
        // UserName дорівнює Email, тому перевизначаємо обидва методи одним повідомленням
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError {
                Code = nameof(DuplicateUserName),
                // Безпечне повідомлення без розкриття самої пошти
                Description = "Не вдалося завершити реєстрацію. Можливо, цей email вже використовується. Спробуйте увійти."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError {
                Code = nameof(DuplicateEmail),
                Description = "Не вдалося завершити реєстрацію. Можливо, цей email вже використовується. Спробуйте увійти."
            };
        }

        // Підтвердження старого паролю
        public override IdentityError PasswordMismatch()
        {
            return new IdentityError {
                Code = nameof(PasswordMismatch),
                Description = "Невірно вказаний поточний пароль (старий)."
            };
        }
    }
}
