using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Exceptions;
using Lacos.GestioneCommesse.Framework.Security;
using Microsoft.EntityFrameworkCore;

namespace Lacos.GestioneCommesse.Application.Security;

public interface ISecurityContext : IDisposable
{
    void ChangeEmailAddress(string? emailAddress);
    Task ChangePassword(string password);
    Task EnableUser(bool enabled);
    Task ChangeUserName(string userName);
    bool VerifyPassword(string password);
    Task GenerateUniqueAccessToken();
    Task HashPasswordWithUniqueSalt(string password);
    Task EnsureUserNameIsUnique(string userName);
    void ChangeName(string? name, string? surname);
    void ChangeColor(string? hex);
}

public class SecurityContext : ISecurityContext
{
    private readonly IPasswordHasher passwordHasher;
    private readonly IAccessTokenGenerator accessTokenGenerator;
    private readonly IQueryable<User> users;
    private readonly User user;

    public SecurityContext(
        IPasswordHasher passwordHasher,
        IAccessTokenGenerator accessTokenGenerator,
        IQueryable<User> users,
        User user
    )
    {
        this.passwordHasher = passwordHasher;
        this.accessTokenGenerator = accessTokenGenerator;
        this.users = users;
        this.user = user;
    }

    public async Task EnableUser(bool enabled)
    {
        if (enabled == user.Enabled)
        {
            return;
        }

        await GenerateUniqueAccessToken();

        user.Enabled = enabled;
    }

    public void ChangeEmailAddress(string? emailAddress)
    {
        user.EmailAddress = emailAddress;
    }

    public async Task ChangePassword(string password)
    {
        var passwordVerified = VerifyPassword(password);

        if (passwordVerified)
        {
            throw new LacosException("La nuova password non può essere uguale a quella vecchia.");
        }

        await HashPasswordWithUniqueSalt(password);
        await GenerateUniqueAccessToken();
    }

    public async Task GenerateUniqueAccessToken()
    {
        bool exists;

        do
        {
            user.AccessToken = accessTokenGenerator.Generate();

            exists = await users
                .AnyAsync(e => e.AccessToken == user.AccessToken);

        } while (exists);
    }

    public bool VerifyPassword(string password)
    {
        var incomingHashedPassword = passwordHasher.HashPassword(password, user.Salt);

        return incomingHashedPassword == user.PasswordHash;
    }

    public async Task ChangeUserName(string userName)
    {
        if (userName == user.UserName)
        {
            return;
        }

        await EnsureUserNameIsUnique(userName);

        user.UserName = userName;
    }

    public async Task HashPasswordWithUniqueSalt(string password)
    {
        bool exists;

        do
        {
            user.Salt = passwordHasher.GenerateSalt();

            exists = await users
                .AnyAsync(e => e.Salt == user.Salt);

        } while (exists);

        user.PasswordHash = passwordHasher.HashPassword(password, user.Salt);
    }

    public async Task EnsureUserNameIsUnique(string userName)
    {
        var exists = await users
            .AnyAsync(e => e.UserName == userName && e.Id != user.Id);

        if (exists)
        {
            throw new LacosException($"Nome utente {userName} già in uso.");
        }
    }

    public void ChangeName(string? name, string? surname)
    {
        user.Name = name;
        user.Surname = surname;
    }

    public void ChangeColor(string? hex)
    {
        user.ColorHex = hex;
    }

    public void Dispose()
    {
    }
}