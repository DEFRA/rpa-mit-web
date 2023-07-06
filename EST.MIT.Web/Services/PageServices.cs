using Helpers;

namespace Services;

public interface IPageServices
{
    bool Validation(dynamic model, out bool IsErrored, out Dictionary<string, string> errors);
}

public class PageServices : IPageServices
{
    public bool Validation(object model, out bool IsErrored, out Dictionary<string, string> errors) => Validate(model, out IsErrored, out errors);

    private static bool Validate<T>(T model, out bool IsErrored, out Dictionary<string, string> errors)
    {
        if (model == null)
        {
            IsErrored = true;
            errors = new();
            return false;
        }

        var validationResults = model.Validate();
        Dictionary<string, string> failures = new();

        if (validationResults.Any())
        {
            IsErrored = true;
            foreach (var result in validationResults)
            {
                failures.Add(result.MemberNames.First().ToLower(), result?.ErrorMessage ?? "Unknown error");
            }
            errors = failures;

            return false;
        }

        IsErrored = false;
        errors = new();
        return true;
    }
}

