using Wayfinder.Core.Data.Definitions;

namespace Wayfinder.Infrastructure.DataValidators
{
    /// <summary>
    /// Validation rules mapping the Class YAML definitions to the ClassDefinition library
    /// </summary>
    public static class ClassSeedValidator
    {
        public static (bool IsValid, List<string> Errors) Validate(ClassDefinition definition)
        {
            var errors = new List<string>();

            if (definition.Levels.Keys.Any(level => level < 1 || level > 20))
            {
                errors.Add($"All classes must have between 1 and 20 levels defined");
            }

            return (errors.Count == 0, errors);
        }
    }
}
