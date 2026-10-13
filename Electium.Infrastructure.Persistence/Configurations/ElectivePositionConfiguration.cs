using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class ElectivePositionConfiguration : IEntityTypeConfiguration<ElectivePosition>
    {
        public void Configure(EntityTypeBuilder<ElectivePosition> builder)
        {
            builder.ToTable("ElectivePositions");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
        }
    }
}
