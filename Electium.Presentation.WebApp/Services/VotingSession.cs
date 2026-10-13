namespace Electium.Presentation.WebApp.Services
{
    // Temporary storage of the citizen's votes during the flow
    public static class VotingSession
    {
        private const string ElectionKey = "Voting.ElectionId";
        private const string CitizenKey = "Voting.CitizenId";
        private const string VotesKey = "Voting.SelectedVotes";
        private const string VerifiedKey = "Voting.IdentityVerified";

        public static void StartSession(ISession session, int electionId, int citizenId)
        {
            session.SetInt32(ElectionKey, electionId);
            session.SetInt32(CitizenKey, citizenId);
            session.SetString(VerifiedKey, "true");
            session.Remove(VotesKey);
        }

        public static int? GetElectionId(ISession session) => session.GetInt32(ElectionKey);
        public static int? GetCitizenId(ISession session) => session.GetInt32(CitizenKey);
        public static bool IsVerified(ISession session) =>
            session.GetString(VerifiedKey) == "true";

        public static void Clear(ISession session)
        {
            session.Remove(ElectionKey);
            session.Remove(CitizenKey);
            session.Remove(VotesKey);
            session.Remove(VerifiedKey);
        }

        public static Dictionary<int, int?> GetVotes(ISession session)
        {
            var raw = session.GetString(VotesKey);
            if (string.IsNullOrWhiteSpace(raw)) return new Dictionary<int, int?>();

            // Format: "positionId:candidateId;positionId:candidateId;" (candidateId empty = none)
            var result = new Dictionary<int, int?>();
            foreach (var pair in raw.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = pair.Split(':');
                if (parts.Length != 2) continue;
                if (!int.TryParse(parts[0], out var posId)) continue;

                int? candidateId = string.IsNullOrEmpty(parts[1])
                    ? null
                    : int.Parse(parts[1]);

                result[posId] = candidateId;
            }

            return result;
        }

        public static void AddVote(ISession session, int positionId, int? candidateId)
        {
            var votes = GetVotes(session);
            votes[positionId] = candidateId;

            var raw = string.Join(";", votes.Select(v => $"{v.Key}:{v.Value}"));
            session.SetString(VotesKey, raw);
        }

        public static List<int> GetCompletedPositionIds(ISession session)
        {
            return GetVotes(session).Keys.ToList();
        }
    }
}
