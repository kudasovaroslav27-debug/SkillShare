using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillShare.Domain.Entities;

namespace SkillShare.DAL.Configurations;

/// <summary>
/// Настройки для сущности 
/// <see cref="UserToken" />
/// </summary>
public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("UserTokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.UserId).IsRequired();

        builder.Property(t => t.RefreshToken).IsRequired().HasMaxLength(512);

        builder.Property(t => t.RefreshTokenExpireTime).IsRequired();

        builder.HasOne(t => t.User)
        .WithOne(u => u.UserToken)
        .HasForeignKey<UserToken>(t => t.UserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.UserId).IsUnique().HasDatabaseName("IX_UserTokens_UserId_Unique");

        builder.HasData(
          new UserToken
          {
              Id = 1,
              RefreshToken = "dasjJH$#HG$@YHDJWSHJD",
              RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7),
              UserId = 1,
          },
          new UserToken
          {
              Id = 2,
              RefreshToken = "kjHG$YH@FDJS$%DHSJ",
              RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7),
              UserId = 2,
          },
          new UserToken
          {
              Id = 3,
              RefreshToken = "mndUF%GFS@JDHFJDJ",
              RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7),
              UserId = 3,
          }
      );
    }
}
