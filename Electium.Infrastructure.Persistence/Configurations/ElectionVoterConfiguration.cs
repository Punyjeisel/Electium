using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class ElectionVoterConfiguration : IEntityTypeConfiguration<ElectionVoter>
    {
        public void Configure(EntityTypeBuilder<ElectionVoter> builder)
        {
            builder.ToTable("ElectionVoters");
            builder.HasKey(ev => ev.Id);

            builder.HasOne(ev => ev.Election)
                .WithMany()
                .HasForeignKey(ev => ev.ElectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ev => ev.Citizen)
                .WithMany()
                .HasForeignKey(ev => ev.CitizenId)
                .OnDelete(DeleteBehavior.Restrict);

            // A citizen can only vote once per election
            builder.HasIndex(ev => new { ev.ElectionId, ev.CitizenId }).IsUnique();
        }
    }
}
