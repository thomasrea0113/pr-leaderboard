using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Leaderboard.Services
{
    public class ClientFormField
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
        public Dictionary<string, string> Attributes;
    }

    /// <summary>
    /// maps property names to their client attributes, in a format that can be easily serialized for the client.
    /// When serialized to json, the dictionary will be parsed to a JSON object, so then the typescript type
    /// can be generic and have an indexer of type 'keyof typeof T'
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class ClientFormFieldAttribuleMap<TModel> : ReadOnlyDictionary<string, ClientFormField>
    {
        public ClientFormFieldAttribuleMap(IDictionary<string, ClientFormField> dictionary) : base(dictionary)
        {
        }

        // A convenient way to access validators for a dynamic object, by providing a member expression like t => t.Property
        public ClientFormField this[Expression<Func<TModel, object>> propertyExpression]
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

    public interface IFormFieldAttributeProvider
    {
        ClientFormFieldAttribuleMap<T> GetFieldAttriutesForModel<T>(bool camelcasePropertyNames = false);
    }

    public class FormFieldAttributeProvider : IFormFieldAttributeProvider
    {
        private readonly IHtmlGenerator _generator;
        private readonly IViewContextGenerator _viewContextGenerator;
        private readonly IModelExpressionProvider _expressionProvider;
        private readonly IModelMetadataProvider _metadataProvider;

        public FormFieldAttributeProvider(
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

        public ClientFormFieldAttribuleMap<T> GetFieldAttriutesForModel<T>(bool camelcasePropertyNames = false)
        {
            var modelState = new ModelStateDictionary();
            var viewData = new ViewDataDictionary<T>(_metadataProvider, modelState);
            var viewContext = _viewContextGenerator.GenerateViewContext<T>();
            var expressionParam = Expression.Parameter(typeof(T), "m");

            // map the methods to the interface, and then get the methods implementation from the class
            var expressionMethod = _expressionProvider.GetType()
                .GetInterfaceMap(typeof(IModelExpressionProvider)).InterfaceMethods
                .Single(m => m.Name == nameof(_expressionProvider.CreateModelExpression));

            var propertyAttributes = new Dictionary<string, ClientFormField>();
            foreach (var prop in typeof(T).GetProperties())
            {
                var propExpression = Expression.Property(expressionParam, prop);
                var expression = Expression.Lambda(propExpression, expressionParam);

                var genericExpressionMethod = expressionMethod
                    .MakeGenericMethod(typeof(T), prop.PropertyType);

                var helper = new InputTagHelper(_generator)
                {
                    For = (ModelExpression)genericExpressionMethod
                        .Invoke(_expressionProvider, new object[] { viewData, expression }),
                    ViewContext = viewContext
                };

                var attrs = new TagHelperAttributeList();
                var tagContext = new TagHelperContext(attrs, new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
                var output = new TagHelperOutput("input", attrs, (_, e) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
                helper.ProcessAsync(tagContext, output);
                propertyAttributes.Add(prop.Name, new ClientFormField
                {
                    TagName = output.TagName,
                    Attributes = output.Attributes.ToDictionary(a => a.Name, a => $"{a.Value}")
                });
            }

            return new ClientFormFieldAttribuleMap<T>(propertyAttributes);
        }
    }
}