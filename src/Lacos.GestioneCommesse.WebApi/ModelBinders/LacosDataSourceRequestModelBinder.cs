using System.Globalization;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure.Implementation;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Lacos.GestioneCommesse.WebApi.ModelBinders;

public class LacosDataSourceModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return context.Metadata.ModelType == typeof(DataSourceRequest)
            ? new BinderTypeModelBinder(typeof(LacosDataSourceRequestModelBinder))
            : null;
    }
}

public class LacosDataSourceRequestAttribute : ModelBinderAttribute
{
    public LacosDataSourceRequestAttribute()
    {
        BinderType = typeof(LacosDataSourceRequestModelBinder);
    }
}

public class LacosDataSourceRequestModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var dataSourceRequest = CreateDataSourceRequest(bindingContext.ModelMetadata, bindingContext.ValueProvider, bindingContext.ModelName);

        bindingContext.Result = ModelBindingResult.Success(dataSourceRequest);

        return Task.CompletedTask;
    }

    private static DataSourceRequest CreateDataSourceRequest(ModelMetadata modelMetadata, IValueProvider valueProvider, string modelName)
    {
        var request = new DataSourceRequest();

        TryGetValue(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.Sort,
            (Action<string>)(sort => request.Sorts = DataSourceDescriptorSerializer.Deserialize<SortDescriptor>(sort)));
        TryGetValue(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.Page, (Action<int>)(currentPage => request.Page = currentPage));
        TryGetValue(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.PageSize, (Action<int>)(pageSize => request.PageSize = pageSize));
        TryGetValue(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.GroupPaging, (Action<bool>)(groupPaging => request.GroupPaging = groupPaging));
        TryGetValue(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.IncludeSubGroupCount,
            (Action<bool>)(includeSubGroupCount => request.IncludeSubGroupCount = includeSubGroupCount));
        TryGetValue(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.Skip, (Action<int>)(skip => request.Skip = skip));
        TryGetValue(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.Take, (Action<int>)(take => request.Take = take));
        TryGetValue(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.Filter, (Action<string>)(filter => request.Filters = LacosFilterDescriptorFactory.Create(filter)));
        TryGetValue<string>(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.Group,
            group => request.Groups = DataSourceDescriptorSerializer.Deserialize<GroupDescriptor>(group));
        TryGetValue<string>(modelMetadata, valueProvider, modelName, DataSourceRequestUrlParameters.Aggregates,
            aggregates => request.Aggregates = DataSourceDescriptorSerializer.Deserialize<AggregateDescriptor>(aggregates));

        return request;
    }

    private static void TryGetValue<T>(ModelMetadata modelMetadata, IValueProvider valueProvider, string modelName, string key, Action<T> action)
    {
        if (modelMetadata.BinderModelName.HasValue())
        {
            key = modelName + "-" + key;
        }

        var result = valueProvider.GetValue(key);

        if (result.FirstValue == null)
        {
            return;
        }

        var obj = result.ConvertValueTo(typeof(T));

        if (obj == null)
        {
            return;
        }

        action((T)obj);
    }
    private static class LacosFilterDescriptorFactory
    {
        public static IList<IFilterDescriptor> Create(string input)
        {
            var filterDescriptorList = new List<IFilterDescriptor>();
            var filterNode = new LacosFilterParser(input).Parse();

            if (filterNode == null)
            {
                return filterDescriptorList;
            }

            var visitor = new FilterNodeVisitor();

            filterNode.Accept(visitor);
            filterDescriptorList.Add(visitor.Result);

            return filterDescriptorList;
        }
    }

    public class LacosFilterParser
    {
        private readonly IList<FilterToken> tokens;
        private int currentTokenIndex;

        public LacosFilterParser(string input) => tokens = new FilterLexer(input).Tokenize();

        public IFilterNode? Parse() => tokens.Count > 0 ? Expression() : null;

        private IFilterNode Expression() => OrExpression();

        private IFilterNode OrExpression()
        {
            var firstArgument = AndExpression();
            if (Is(FilterTokenType.Or))
                return ParseOrExpression(firstArgument);
            if (!Is(FilterTokenType.And))
                return firstArgument;
            Expect(FilterTokenType.And);
            return new AndNode
            {
                First = firstArgument,
                Second = OrExpression()
            };
        }

        private IFilterNode AndExpression()
        {
            var firstArgument = ComparisonExpression();
            return Is(FilterTokenType.And) ? ParseAndExpression(firstArgument) : firstArgument;
        }

        private IFilterNode ComparisonExpression()
        {
            var firstArgument = PrimaryExpression();
            return Is(FilterTokenType.ComparisonOperator) || Is(FilterTokenType.Function) ? ParseComparisonExpression(firstArgument) : firstArgument;
        }

        private IFilterNode PrimaryExpression()
        {
            if (Is(FilterTokenType.LeftParenthesis))
                return ParseNestedExpression();
            if (Is(FilterTokenType.Function))
                return ParseFunctionExpression();
            if (Is(FilterTokenType.Boolean))
                return ParseBoolean();
            if (Is(FilterTokenType.DateTime))
                return ParseDateTimeExpression();
            if (Is(FilterTokenType.Property))
                return ParsePropertyExpression();
            if (Is(FilterTokenType.Number))
                return ParseNumberExpression();
            if (Is(FilterTokenType.String))
                return ParseStringExpression();
            if (Is(FilterTokenType.Null))
                return ParseNullExpression();
            throw new FilterParserException("Expected primaryExpression");
        }

        private IFilterNode ParseOrExpression(IFilterNode firstArgument)
        {
            Expect(FilterTokenType.Or);
            var filterNode = OrExpression();
            return new OrNode
            {
                First = firstArgument,
                Second = filterNode
            };
        }

        private IFilterNode ParseComparisonExpression(IFilterNode firstArgument)
        {
            if (Is(FilterTokenType.ComparisonOperator))
            {
                var token = Expect(FilterTokenType.ComparisonOperator);
                var filterNode = PrimaryExpression();
                return new ComparisonNode
                {
                    First = firstArgument,
                    FilterOperator = token.ToFilterOperator(),
                    Second = filterNode
                };
            }

            var token1 = Expect(FilterTokenType.Function);
            var comparisonExpression = new FunctionNode();
            comparisonExpression.FilterOperator = token1.ToFilterOperator();
            comparisonExpression.Arguments.Add(firstArgument);
            comparisonExpression.Arguments.Add(PrimaryExpression());
            return comparisonExpression;
        }

        private IFilterNode ParseAndExpression(IFilterNode firstArgument)
        {
            Expect(FilterTokenType.And);
            var filterNode = ComparisonExpression();
            return new AndNode
            {
                First = firstArgument,
                Second = filterNode
            };
        }

        private IFilterNode ParseNullExpression()
        {
            var filterToken = Expect(FilterTokenType.Null);
            return new NullNode
            {
                Value = filterToken.Value
            };
        }

        private IFilterNode ParseStringExpression()
        {
            var filterToken = Expect(FilterTokenType.String);
            return new StringNode
            {
                Value = filterToken.Value
            };
        }

        private IFilterNode ParseBoolean()
        {
            var filterToken = Expect(FilterTokenType.Boolean);
            return new BooleanNode
            {
                Value = Convert.ToBoolean(filterToken.Value)
            };
        }

        private IFilterNode ParseNumberExpression()
        {
            var filterToken = Expect(FilterTokenType.Number);
            return new NumberNode
            {
                Value = Convert.ToDouble(filterToken.Value, CultureInfo.InvariantCulture)
            };
        }

        private IFilterNode ParsePropertyExpression()
        {
            var filterToken = Expect(FilterTokenType.Property);
            return new PropertyNode
            {
                Name = filterToken.Value
            };
        }

        private IFilterNode ParseDateTimeExpression()
        {
            var filterToken = Expect(FilterTokenType.DateTime);
            return new DateTimeNode
            {
                Value = DateTimeOffset.Parse(filterToken.Value?.Replace(" ", "+"))
            };
        }

        private IFilterNode ParseNestedExpression()
        {
            Expect(FilterTokenType.LeftParenthesis);
            var nestedExpression = Expression();
            Expect(FilterTokenType.RightParenthesis);
            return nestedExpression;
        }

        private IFilterNode ParseFunctionExpression()
        {
            var token = Expect(FilterTokenType.Function);
            var functionExpression = new FunctionNode
            {
                FilterOperator = token.ToFilterOperator()
            };
            Expect(FilterTokenType.LeftParenthesis);
            functionExpression.Arguments.Add(Expression());
            while (Is(FilterTokenType.Comma))
            {
                Expect(FilterTokenType.Comma);
                functionExpression.Arguments.Add(Expression());
            }

            Expect(FilterTokenType.RightParenthesis);
            return functionExpression;
        }

        private FilterToken Expect(FilterTokenType tokenType)
        {
            if (!Is(tokenType))
                throw new FilterParserException("Expected " + tokenType);
            var filterToken = Peek();
            ++currentTokenIndex;
            return filterToken;
        }

        private bool Is(FilterTokenType tokenType)
        {
            var filterToken = Peek();
            return filterToken != null && filterToken.TokenType == tokenType;
        }

        private FilterToken Peek() => currentTokenIndex < tokens.Count ? tokens[currentTokenIndex] : null;
    }
}