using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SMS.Data.Entities;
using SMS.Web;

namespace SMS.Web.Models;

public class UserProfileViewModel
{    
    public int Id { get; set; }

    [Required] public string Name { get; set; }  

    [Required] [EmailAddress]
    [Remote("VerifyEmailAddress", "User", AdditionalFields = nameof(Id))]
    public string Email { get; set; }
    
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
    [Display(Name = "Confirm Password")]  
    public string PasswordConfirm  { get; set; }

    [Required] public Role Role { get; set; }

    public IFormFile Avatar { get; set; }

}


