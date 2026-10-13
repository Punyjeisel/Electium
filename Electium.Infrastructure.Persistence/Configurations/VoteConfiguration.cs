using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.ToTable("Votes");
            builder.HasKey(v => v.Id);

            builder.HasOne(v => v.Election)
                .WithMany()
                .HasForeignKey(v => v.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.ElectivePosition)
                .WithMany()
                .HasForeignKey(v => v.ElectivePositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // CandidateId can be null (the "Ninguno" option)
            builder.HasOne(v => v.Candidate)
                .WithMany()
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
