using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class CandidatePositionConfiguration : IEntityTypeConfiguration<CandidatePosition>
    {
        public void Configure(EntityTypeBuilder<CandidatePosition> builder)
        {
            builder.ToTable("CandidatePositions");
            builder.HasKey(cp => cp.Id);

            builder.HasOne(cp => cp.PoliticalParty)
                .WithMany()
                .HasForeignKey(cp => cp.PoliticalPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.Candidate)
                .WithMany()
                .HasForeignKey(cp => cp.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.ElectivePosition)
                .WithMany()
                .HasForeignKey(cp => cp.ElectivePositionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
