using Lacos.GestioneCommesse.Application.Security;
using Lacos.GestioneCommesse.Application.Security.DTOs;
using Lacos.GestioneCommesse.WebApi.Auth;
using Lacos.GestioneCommesse.WebApi.Models.Security;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lacos.GestioneCommesse.WebApi.Controllers;

[RequireUser]
public class SecurityController : LacosApiController
{
    private readonly ISecurityService service;

    public SecurityController(ISecurityService service)
    {
        this.service = service;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<UserDto> Login(LoginModel model)
    {
        return await service.Login(model.UserName, model.Password);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok();
    }

    [HttpGet("users")]
    public async Task<DataSourceResult> GetUsers([DataSourceRequest] DataSourceRequest request)
    {
        var result = await service.Query()
            .ToDataSourceResultAsync(request);

        return result;
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(UpdateUserRequest request)
    {
        var user = new UserDto(request.UserName, request.Enabled, request.Role, null, null);
        var result = await service.Register(user, request.Password);

        return CreatedAtAction(nameof(GetUser), new { result.Id }, result);
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(long id, UpdateUserRequest request)
    {
        await service.ExecuteOnUser(id,
            async context =>
            {
                await context.ChangeUserName(request.UserName);

                if (!string.IsNullOrEmpty(request.Password) && !context.VerifyPassword(request.Password))
                {
                    await context.ChangePassword(request.Password);
                }

                await context.EnableUser(request.Enabled);
            });

        return NoContent();
    }

    [HttpGet("users/{id}")]
    public async Task<UserDto> GetUser(long id)
    {
        var user = await service.GetUser(id);

        return user;
    }

    [HttpGet("users/current")]
    public async Task<UserDto> GetCurrentUser()
    {
        return await service.GetCurrentUser();
    }

    [HttpPost("users/current/changepassword")]
    public async Task<ActionResult<UserDto>> ChangeCurrentUserPassword(ChangeUserPasswordModel model)
    {
        var user = await service.ChangeCurrentUserPassword(model.CurrentPassword, model.NewPassword);

        return user;
    }
}
