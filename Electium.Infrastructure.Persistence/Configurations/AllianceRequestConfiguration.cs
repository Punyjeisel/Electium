using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class AllianceRequestConfiguration : IEntityTypeConfiguration<AllianceRequest>
    {
        public void Configure(EntityTypeBuilder<AllianceRequest> builder)
        {
            builder.ToTable("AllianceRequests");
            builder.HasKey(ar => ar.Id);

            // Two relationships to PoliticalParty - must be configured explicitly
            builder.HasOne(ar => ar.RequesterParty)
                .WithMany()
                .HasForeignKey(ar => ar.RequesterPartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ar => ar.ReceiverParty)
                .WithMany()
                .HasForeignKey(ar => ar.ReceiverPartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
