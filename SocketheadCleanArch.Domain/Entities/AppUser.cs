using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SocketheadCleanArch.Domain.Entities;

public class AppUser : IdentityUser<string>
{
    [StringLength(200)]
    [PersonalData]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [StringLength(200)]
    [PersonalData]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [NotMapped] public string FullName => $"{FirstName} {LastName}".Trim(); 
}