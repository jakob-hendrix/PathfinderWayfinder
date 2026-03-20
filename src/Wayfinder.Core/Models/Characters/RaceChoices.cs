namespace Wayfinder.Core.Models.Characters;

public class RaceChoices
{
    public string RaceName { get; set; } = string.Empty;
    public string? SubraceName { get; set; }
    public List<string> SelectedAlternativeTraits { get; set; } = new();
}
