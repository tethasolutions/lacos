using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Lacos.GestioneCommesse.WebApi.ModelBinders;

public class DateTimeOffsetModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        value = value.Replace(" ", "+");

        if (!DateTimeOffset.TryParse(value, out var dateTimeOffset))
        {
            bindingContext.ModelState.TryAddModelError(modelName, "DateTimeOffset invalid.");
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Success(dateTimeOffset);
        }

        return Task.CompletedTask;
    }
}

public class DateTimeOffsetModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return context.Metadata.ModelType == typeof(DateTimeOffset)
            ? new BinderTypeModelBinder(typeof(DateTimeOffsetModelBinder))
            : null;
    }
}