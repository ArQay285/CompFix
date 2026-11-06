using System.ComponentModel.DataAnnotations;

namespace CompFix.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Введіть логін!")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль!")]
        [DataType(DataType.Password)] // Вказує, що поле є паролем і має відображатися як прихований текст
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }
    }
}
