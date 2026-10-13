using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.ToTable("Candidates");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.PhotoPath).IsRequired().HasMaxLength(300);

            // One candidate belongs to one party
            builder.HasOne(c => c.PoliticalParty)
                .WithMany()
                .HasForeignKey(c => c.PoliticalPartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
