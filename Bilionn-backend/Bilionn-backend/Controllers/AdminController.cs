using Bilionn_backend.DTOs.Auth;
using Bilionn_backend.Models;
using Bilionn_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bilionn_backend.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminController: ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService) => _userService = userService;

    [HttpGet("users")]
    public async Task<ActionResult<List<UserResponse>>> GetUsers() =>
        Ok(await _userService.GetAllAsync());
}
