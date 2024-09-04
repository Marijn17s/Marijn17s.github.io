namespace RaceFlowAPI.Models;

public class Race
{
    public int Id { get; set; }
    public int Laps { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public IEnumerable<Driver> Participants { get; set; }
}