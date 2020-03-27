using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Leaderboard.Data;
using Leaderboard.Models.Features;
using Microsoft.Extensions.DependencyInjection;
using static Leaderboard.Utilities.SlugUtilities;

namespace Leaderboard.Areas.Leaderboards.Validators
{
    public class ValidSlugAttribute : ValidationAttribute
    {
        private Type _modelType;

        private void Construct(Type modelType)
        {
            if (modelType is null)
                throw new ArgumentNullException(nameof(modelType));
            if (!typeof(ISlugged).IsAssignableFrom(modelType))
                throw new ArgumentException($"type {modelType} must implement {typeof(ISlugged)}");

            _modelType = modelType;
        }

        public ValidSlugAttribute(Type modelType)
        {
            Construct(modelType);
        }

        public ValidSlugAttribute(Type modelType, Func<string> errorMessageAccessor) : base(errorMessageAccessor)
        {
            Construct(modelType);
        }

        public ValidSlugAttribute(Type modelType, string errorMessage) : base(errorMessage)
        {
            Construct(modelType);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string str && IsSlug(str))
            {
                var ctx = validationContext.GetRequiredService<ApplicationDbContext>();
                var set = ctx.Query<ISlugged>(_modelType);

                if (set.Any(e => e.Slug == str))
                    return ValidationResult.Success;
            }
            return new ValidationResult(FormatErrorMessage(value?.ToString()));
        }
    }
}
