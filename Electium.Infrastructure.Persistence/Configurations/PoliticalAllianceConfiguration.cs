using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class PoliticalAllianceConfiguration : IEntityTypeConfiguration<PoliticalAlliance>
    {
        public void Configure(EntityTypeBuilder<PoliticalAlliance> builder)
        {
            builder.ToTable("PoliticalAlliances");
            builder.HasKey(pa => pa.Id);

            builder.HasOne(pa => pa.PartyA)
                .WithMany()
                .HasForeignKey(pa => pa.PartyAId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pa => pa.PartyB)
                .WithMany()
                .HasForeignKey(pa => pa.PartyBId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
