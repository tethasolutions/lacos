using System.Reflection;
using Lacos.GestioneCommesse.Framework.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lacos.GestioneCommesse.WebApi.Auth;

public class AuthorizeFilter : IAuthorizationFilter
{
    private readonly ILacosSession session;

    public AuthorizeFilter(ILacosSession session)
    {
        this.session = session;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor is not ControllerActionDescriptor descriptor)
        {
            return;
        }

        var action = new AuthorizationDescriptor(descriptor.MethodInfo);
        var controller = new AuthorizationDescriptor(descriptor.ControllerTypeInfo);
        var allowAnonymous = action.AllowAnonymous ||
                             (controller.AllowAnonymous && action is { RequireUser: false, RequireRoles: false });

        if (allowAnonymous)
        {
            return;
        }

        var requireUser = action.RequireUser || controller.RequireUser;
        var requireRoles = action.RequireRoles || controller.RequireRoles;

        if (!requireRoles && !requireUser)
        {
            return;
        }

        if (requireUser && !requireRoles)
        {
            var isAuthenticated = session.IsAuthenticated();

            if (!isAuthenticated)
            {
                context.Result = new UnauthorizedResult();
            }

            return;
        }

        var requiredRoles = action.RequireRoles
            ? action.RequireRoleAttributes
            : controller.RequireRoleAttributes;
        var isAuthorized = requiredRoles
            .Any(e => session.IsAuthorized(e.Role));

        if (!isAuthorized)
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private class AuthorizationDescriptor
    {
        public bool AllowAnonymous { get; }
        public bool RequireUser { get; }
        public bool RequireRoles => RequireRoleAttributes.Any();
        public ICollection<RequireRoleAttribute> RequireRoleAttributes { get; }

        public AuthorizationDescriptor(MemberInfo info)
        {
            AllowAnonymous = info.GetCustomAttribute<AllowAnonymousAttribute>() != null;
            RequireUser = info.GetCustomAttribute<RequireUserAttribute>() != null;
            RequireRoleAttributes = info.GetCustomAttributes<RequireRoleAttribute>().ToList();
        }
    }
}