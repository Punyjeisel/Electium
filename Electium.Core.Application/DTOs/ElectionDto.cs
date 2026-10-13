using Electium.Core.Domain.Enums;

namespace Electium.Core.Application.DTOs;

public class ElectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public ElectionStatus Status { get; set; }
    public int PartyCount { get; set; }
    public int PositionCount { get; set; }
    public int TotalVotes { get; set; }
}