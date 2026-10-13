using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class PoliticalPartyConfiguration : IEntityTypeConfiguration<PoliticalParty>
    {
        public void Configure(EntityTypeBuilder<PoliticalParty> builder)
        {
            builder.ToTable("PoliticalParties");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.Acronym).IsRequired().HasMaxLength(20);
            builder.Property(p => p.LogoPath).IsRequired().HasMaxLength(300);

            // Acronym is unique
            builder.HasIndex(p => p.Acronym).IsUnique();
        }
    }
}
