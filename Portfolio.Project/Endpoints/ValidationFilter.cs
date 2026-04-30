using System.ComponentModel.DataAnnotations;

namespace Portfolio.Project.Endpoints;

public static class ValidationFilter
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var argument = context.Arguments.OfType<T>().FirstOrDefault();
            if (argument is not null)
            {
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(
                    argument, new ValidationContext(argument), results, validateAllProperties: true);

                if (!isValid)
                {
                    var errors = results
                        .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(r => r.ErrorMessage ?? string.Empty).ToArray());

                    return Results.ValidationProblem(errors);
                }
            }

            return await next(context);
        });
    }
}
