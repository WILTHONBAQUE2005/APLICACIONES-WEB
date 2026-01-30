using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SalesPro.Web.Binding;

public sealed class DecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var raw = valueProviderResult.FirstValue;
        if (string.IsNullOrWhiteSpace(raw)) return Task.CompletedTask;

        raw = raw.Trim();

        if (TryParse(raw, bindingContext.ModelMetadata.IsNullableValueType, out var parsed))
        {
            bindingContext.Result = ModelBindingResult.Success(parsed);
            return Task.CompletedTask;
        }

        bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Valor numérico inválido.");
        return Task.CompletedTask;
    }

    private static bool TryParse(string raw, bool isNullable, out object? result)
    {
        result = null;

        if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out var dec))
        {
            result = dec;
            return true;
        }

        if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out dec))
        {
            result = dec;
            return true;
        }

        var swapped = SwapSeparators(raw);

        if (decimal.TryParse(swapped, NumberStyles.Number, CultureInfo.CurrentCulture, out dec))
        {
            result = dec;
            return true;
        }

        if (decimal.TryParse(swapped, NumberStyles.Number, CultureInfo.InvariantCulture, out dec))
        {
            result = dec;
            return true;
        }

        if (isNullable && string.IsNullOrWhiteSpace(raw))
        {
            result = null;
            return true;
        }

        return false;
    }

    private static string SwapSeparators(string input)
    {
        if (input.Contains('.') && !input.Contains(',')) return input.Replace('.', ',');
        if (input.Contains(',') && !input.Contains('.')) return input.Replace(',', '.');
        return input;
    }
}
