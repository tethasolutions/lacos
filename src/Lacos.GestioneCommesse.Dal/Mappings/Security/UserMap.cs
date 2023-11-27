using Lacos.GestioneCommesse.Dal.Extensions;
using Lacos.GestioneCommesse.Domain.Security;
using Lacos.GestioneCommesse.Framework.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacos.GestioneCommesse.Dal.Mappings.Security;

public class UserMap : BaseEntityMapping<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users", "Security");

        builder.Property(e => e.UserName)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(e => e.Salt)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(e => e.AccessToken)
            .IsRequired()
            .HasMaxLength(64);

        builder.OneToOne(e => e.Operator, e => e.User, e => e.UserId);
        builder.OneToOne(e => e.Customer, e => e.User, e => e.UserId);

        builder.HasData(GetData());
    }

    private static User[] GetData()
    {
        var passwordHasher = new PasswordHasher();

        return new[]
        {
            new User
            {
                Id = 1,
                UserName = "administrator",
                AccessToken = "a0f0a2ffd0f37c955fda023ed287c12fab375bfc0c3e58f96114c9eeb20066b0",
                Enabled = true,
                PasswordHash = passwordHasher.HashPassword("l@c0s@dm1n", "f3064d73de0ca6b806ad24df65a59e1eb692393fc3f0b0297e37df522610b58b"),
                Role = Role.Administrator,
                Salt = "f3064d73de0ca6b806ad24df65a59e1eb692393fc3f0b0297e37df522610b58b"
            }
        };
    }
}
