using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sockethead.Razor.Alert.Extensions;
using SocketheadCleanArch.Admin.Extensions;
using SocketheadCleanArch.Admin.Models;
using SocketheadCleanArch.Domain.Entities;
using SocketheadCleanArch.Service.Repository;

namespace SocketheadCleanArch.Admin.Controllers;

[Authorize(Roles = "Admin" /*, Policy = "RequireMFA")*/)]
public class UserAdminController(UserAdminRepository userAdminRepo) : Controller
{
    public IActionResult Users()
    {
        this.SetTitle("Users");
        
        return View(userAdminRepo.Users);
    }

    [HttpGet]
    public async Task<IActionResult> EditUserRoles(string userId)
    {
        this.SetTitle("Edit User Roles");
        
        AppUser? user = await userAdminRepo.FindUserByIdAsync(userId);
        if (user == null) 
            return NotFound();

        IReadOnlyList<string> userRoles = await userAdminRepo.GetUserRolesAsync(user);
        List<AppRole> allRoles = await userAdminRepo.Roles.ToListAsync();

        return View(new EditUserRolesVM
        {
            UserId = user.Id,
            UserName = user.UserName,
            AssignedRoles = userRoles,
            AvailableRoles = allRoles.Select(r => r.Name).ToList()
        });
    }

    [HttpPost]
    public async Task<IActionResult> EditUserRoles(EditUserRolesVM model)
    {
        AppUser? user = await userAdminRepo.FindUserByIdAsync(model.UserId);
        if (user == null) 
            return NotFound();

        await userAdminRepo.SetRolesAsync(user, model.SelectedRoles);
        
        return RedirectToAction(nameof(Users)).Success("Successfully updated user roles.");
    }
}