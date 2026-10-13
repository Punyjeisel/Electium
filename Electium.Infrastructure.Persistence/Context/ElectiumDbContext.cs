using Electium.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Electium.Infrastructure.Persistence.Context
{
    public class ElectiumDbContext : DbContext
    {
        public ElectiumDbContext(DbContextOptions<ElectiumDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Citizen> Citizens => Set<Citizen>();
        public DbSet<PoliticalParty> PoliticalParties => Set<PoliticalParty>();
        public DbSet<ElectivePosition> ElectivePositions => Set<ElectivePosition>();
        public DbSet<Candidate> Candidates => Set<Candidate>();
        public DbSet<PartyLeader> PartyLeaders => Set<PartyLeader>();
        public DbSet<CandidatePosition> CandidatePositions => Set<CandidatePosition>();
        public DbSet<AllianceRequest> AllianceRequests => Set<AllianceRequest>();
        public DbSet<PoliticalAlliance> PoliticalAlliances => Set<PoliticalAlliance>();
        public DbSet<Election> Elections => Set<Election>();
        public DbSet<ElectionCandidate> ElectionCandidates => Set<ElectionCandidate>();
        public DbSet<ElectionVoter> ElectionVoters => Set<ElectionVoter>();
        public DbSet<Vote> Votes => Set<Vote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Load all configurations from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElectiumDbContext).Assembly);
        }
    }
}
