using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class ElectionCandidateConfiguration : IEntityTypeConfiguration<ElectionCandidate>
    {
        public void Configure(EntityTypeBuilder<ElectionCandidate> builder)
        {
            builder.ToTable("ElectionCandidates");
            builder.HasKey(ec => ec.Id);

            builder.HasOne(ec => ec.Election)
                .WithMany()
                .HasForeignKey(ec => ec.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ec => ec.ElectivePosition)
                .WithMany()
                .HasForeignKey(ec => ec.ElectivePositionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ec => ec.Candidate)
                .WithMany()
                .HasForeignKey(ec => ec.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ec => ec.PoliticalParty)
                .WithMany()
                .HasForeignKey(ec => ec.PoliticalPartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
