using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SalesPro.Web.Binding;

public sealed class DecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?))
            return new DecimalModelBinder();

        return null;
    }
}
