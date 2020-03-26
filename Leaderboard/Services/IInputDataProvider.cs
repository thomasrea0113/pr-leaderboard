using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Leaderboard.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Leaderboard.Services
{
    public class ClientValidationField
    {
        /// <summary>
        /// The client HTML tag name
        /// </summary>
        /// <value></value>
        public string TagName { get; set; }

        /// <summary>
        /// The html attributes to apply to to the tag
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
    }

    /// <summary>
    /// maps property names to their client attributes, in a format that can be easily serialized for the client
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class ClientValidationModelMap<TModel> : ReadOnlyDictionary<string, ClientValidationField>
    {
        public ClientValidationModelMap(IDictionary<string, ClientValidationField> dictionary) : base(dictionary)
        {
        }

        // A convenient way to access validators for a dynamic object, by providing a member expression like t => t.Property
        public ClientValidationField this[Expression<Func<TModel, object>> propertyExpression]
        {
            get
            {
                if (!(propertyExpression.Body is MemberExpression member))
                    // The property access might be getting converted to object to match the func
                    // If so, get the operand and see if that's a member expression
                    member = (propertyExpression.Body as UnaryExpression)?.Operand as MemberExpression;
                if (member == null)
                    throw new ArgumentException("Action must be a member expression.");
                return this[member.Member.Name];
            }
        }
    }

    public interface IInputDataProvider
    {
        object GetFieldAttriutesForModel<T>();
    }

    public class InputDataProvider : IInputDataProvider
    {
        private readonly IHtmlGenerator _generator;
        private readonly IViewContextGenerator _viewContextGenerator;
        private readonly IModelExpressionProvider _expressionProvider;
        private readonly IModelMetadataProvider _metadataProvider;

        public InputDataProvider(
            IModelMetadataProvider metadataProvider,
            IModelExpressionProvider expressionProvider,
            IHtmlGenerator generator,
            IViewContextGenerator viewContextGenerator)
        {
            _generator = generator;
            _viewContextGenerator = viewContextGenerator;
            _expressionProvider = expressionProvider;
            _metadataProvider = metadataProvider;
        }

        public object GetFieldAttriutesForModel<T>()
        {
            var modelState = new ModelStateDictionary();
            var viewData = new ViewDataDictionary<T>(_metadataProvider, modelState);
            var viewContext = _viewContextGenerator.GenerateViewContext<T>();
            var expressionParam = Expression.Parameter(typeof(ContactViewModel), "m");


            var propertyAttributes = new Dictionary<string, ClientValidationField>();
            foreach (var prop in typeof(T).GetProperties())
            {
                var propExpression = Expression.Property(expressionParam, prop);
                var expression = Expression.Lambda<Func<T, object>>(propExpression, expressionParam);

                var helper = new InputTagHelper(_generator)
                {
                    For = _expressionProvider.CreateModelExpression(viewData, expression),
                    ViewContext = viewContext
                };

                var attrs = new TagHelperAttributeList();
                var tagContext = new TagHelperContext(attrs, new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
                var output = new TagHelperOutput("input", attrs, (_, e) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
                helper.ProcessAsync(tagContext, output);
                propertyAttributes.Add(prop.Name, new ClientValidationField
                {
                    TagName = output.TagName,
                    Attributes = output.Attributes.ToDictionary(a => a.Name, a => $"{a.Value}")
                });
            }

            return new ClientValidationModelMap<T>(propertyAttributes);
        }
    }
}
