using System.ComponentModel.DataAnnotations;

namespace CompFix.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введіть ім'я!")]
        [Display(Name = "ім'я")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ' -]+$", ErrorMessage = "Ім'я може містити лише літери, пробіл або дефіс.")]
        [StringLength(60, ErrorMessage = "Ім'я не може бути довшим за 60 символів.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть Призвище!")]
        [Display(Name = "Призвище")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ' -]+$", ErrorMessage = "Призвище може містити лише літери, пробіл або дефіс.")]
        [StringLength(60, ErrorMessage = "Призвище не може бути довшим за 60 символів.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть електронну пошту!")]
        [EmailAddress(ErrorMessage = "Некоректний формат пошти")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль!")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути не менше 6 символів (до 100)")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        [Compare("Password", ErrorMessage = "Паролі не збігаються")] 
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
