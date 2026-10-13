using Electium.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Persistence.Configurations
{
    public class PartyLeaderConfiguration : IEntityTypeConfiguration<PartyLeader>
    {
        public void Configure(EntityTypeBuilder<PartyLeader> builder)
        {
            builder.ToTable("PartyLeaders");
            builder.HasKey(pl => pl.Id);

            builder.HasOne(pl => pl.User)
                .WithMany()
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pl => pl.PoliticalParty)
                .WithMany()
                .HasForeignKey(pl => pl.PoliticalPartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
