using System.ComponentModel.DataAnnotations;

namespace CompFix.ViewModels
{
    public class UserSettingsViewModel
    {
        [Required(ErrorMessage = "Введіть ім'я!")]
        [Display(Name = "ім'я")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ' -]+$", ErrorMessage = "Ім'я може містити лише літери, пробіл або дефіс.")]
        [StringLength(60, ErrorMessage = "Ім'я не може бути довшим за 60 символів.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть Прізвище!")]
        [Display(Name = "Прізвище")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ' -]+$", ErrorMessage = "Прізвище може містити лише літери, пробіл або дефіс.")]
        [StringLength(60, ErrorMessage = "Прізвище не може бути довшим за 60 символів.")]
        public string LastName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Поточний пароль (старий)")]
        public string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Новий пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути не менше 6 символів (до 100)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Паролі не збігаються")]
        public string? ConfirmNewPassword { get; set; }
    }
}
