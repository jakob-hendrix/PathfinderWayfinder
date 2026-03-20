namespace Wayfinder.Core.Models.Characters;

public class Subrace
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // TODO: change this to a list of string - the factory can do the replacement and
    // flesh out the trait, since all subrace traits must match an alt trait already on the 
    public List<AlternativeRacialTrait> RacialTraits { get; init; } = new();
}
