using EST.MIT.Web.Helpers;
using EST.MIT.Web.Interfaces;

namespace EST.MIT.Web.Services;

public class PageServices : IPageServices
{
    public bool Validation(object model, out bool IsErrored, out Dictionary<string, List<string>> errors) => Validate(model, out IsErrored, out errors);

    private static bool Validate<T>(T model, out bool IsErrored, out Dictionary<string, List<string>> errors)
    {
        if (model == null)
        {
            IsErrored = true;
            errors = new();
            return false;
        }

        var validationResults = model.Validate();
        Dictionary<string, List<string>> failures = new();

        if (validationResults.Any())
        {
            IsErrored = true;
            foreach (var result in validationResults)
            {
                if (failures.ContainsKey(result.MemberNames.First().ToLower()))
                {
                    failures[result.MemberNames.First().ToLower()].Add(result?.ErrorMessage ?? "Unknown error");
                    continue;
                }

                failures.Add(result.MemberNames.First().ToLower(), new List<string> { result?.ErrorMessage ?? "Unknown error" });
            }
            errors = failures;

            return false;
        }

        IsErrored = false;
        errors = new();
        return true;
    }
}