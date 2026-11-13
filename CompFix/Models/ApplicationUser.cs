using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CompFix.Models
{
    // Успадковує від IdentityUser, щоб зберегти базові поля (Email, PasswordHash тощо)
    public class ApplicationUser : IdentityUser
    {
        [PersonalData] // Вказує Identity, що це особисті дані користувача
        [StringLength(60, ErrorMessage = "Ім'я не може бути довшим за 60 символів.")]
        public string? FirstName { get; set; }

        [PersonalData] // Вказує Identity, що це особисті дані користувача
        [StringLength(60, ErrorMessage = "Прізвище не може бути довшим за 60 символів.")]
        public string? LastName { get; set; }
    }
}
