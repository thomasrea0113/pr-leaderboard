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
    /// <summary>
    /// a mapping of html attributes keys and values. Stubbed as a class
    /// in case we eventually want to pass down more than just attributes
    /// </summary>
    public class ClientFormField : Dictionary<string, string>
    {
        public ClientFormField(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }
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
                propertyAttributes.Add(prop.Name, new ClientFormField(
                    output.Attributes.ToDictionary(a => a.Name, a => $"{a.Value}")));
            }

            return new ClientFormFieldAttribuleMap<T>(propertyAttributes);
        }
    }
}
