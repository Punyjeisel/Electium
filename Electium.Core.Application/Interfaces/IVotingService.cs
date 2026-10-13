using Electium.Core.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Core.Application.Interfaces
{
    public interface IVotingService
    {
        Task<VoterCheckDto> CheckVoterAsync(string identityNumber);
        Task<bool> VerifyIdentityAsync(string manualIdentityNumber, string ocrIdentityNumber);
        Task<List<VotingPositionDto>> GetPositionsAsync(int electionId);
        Task<List<VotingCandidateDto>> GetCandidatesByPositionAsync(int electionId, int positionId);
        Task RegisterVoteAsync(RegisterVoteDto dto);
        Task<VotingSummaryDto> CompleteVotingAsync(int electionId, int citizenId);
    }
}
