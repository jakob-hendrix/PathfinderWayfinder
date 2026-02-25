using Wayfinder.Core.Data.Definitions;

namespace Wayfinder.Infrastructure.DataValidators
{
    /// <summary>
    /// Validation rules mapping the Race YAML definitions to the ClassDefinition library
    /// </summary>
    public static class RaceSeedValidator
    {
        public static (bool IsValid, List<string> Errors) Validate(RaceDefinition definition)
        {
            var errors = new List<string>();

            // TODO: add race definition validations

            return (errors.Count == 0, errors);
        }
    }
}
