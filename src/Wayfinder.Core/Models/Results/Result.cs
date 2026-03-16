namespace Wayfinder.Core.Models.Results;

public abstract class Result
{
    public List<string> Errors { get; } = new();
    public bool IsValid => !Errors.Any();
    public void AddError(string error) => Errors.Add(error);
}
