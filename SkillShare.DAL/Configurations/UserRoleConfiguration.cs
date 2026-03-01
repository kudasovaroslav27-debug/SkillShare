using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SkillShare.Domain.Entities;
using Org.BouncyCastle.Crypto.Generators;

namespace SkillShare.DAL.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Property(ur => ur.UserId).IsRequired();
        builder.Property(ur => ur.RoleId).IsRequired();
        builder.HasIndex(ur => ur.RoleId);

        builder.HasData(
           new UserRole { UserId = 1, RoleId = 1 },
           new UserRole { UserId = 2, RoleId = 2 }, 
           new UserRole { UserId = 3, RoleId = 3 }  
       );
    }
}
