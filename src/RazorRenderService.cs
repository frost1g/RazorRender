using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorRender
{
    public class RazorRenderService : ViewExecutor, IRazorRenderService
    {
        private readonly IActionContextAccessor _ActionContextAccessor;
        private ITempDataProvider _TempDataProvider;

        public RazorRenderService(
            IActionContextAccessor actionContextAccessor,
            IOptions<MvcViewOptions> viewOptions,
            IHttpResponseStreamWriterFactory writerFactory,
            ICompositeViewEngine viewEngine,
            ITempDataDictionaryFactory tempDataFactory,
            DiagnosticSource diagnosticSource,
            IModelMetadataProvider modelMetadataProvider,
            ITempDataProvider tempDataProvider)
            : base(viewOptions, writerFactory, viewEngine, tempDataFactory, diagnosticSource, modelMetadataProvider)
        {
            _ActionContextAccessor = actionContextAccessor;
            _TempDataProvider = tempDataProvider;
        }

        public async Task<string> ViewToStringAsync(string viewName)
        {
            return await ViewToStringAsync<object>(viewName, null);
        }

        public async Task<string> ViewToStringAsync<T>(string viewName, T model)
        {
            var context = _ActionContextAccessor.ActionContext;

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var result = new ViewResult()
            {
                ViewData = new ViewDataDictionary(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                {
                    Model = model,
                },
                TempData = new TempDataDictionary(
                        context.HttpContext,
                        _TempDataProvider),
                ViewName = viewName,
            };

            if (ViewEngine == null)
                throw new ArgumentNullException(nameof(ViewEngine));

            var viewEngineResult = ViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);

            if (viewEngineResult.View == null)
                throw new ArgumentNullException(nameof(viewEngineResult.View));

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    context,
                    viewEngineResult.View,
                    new ViewDataDictionary(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        context.HttpContext,
                        _TempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await viewEngineResult.View.RenderAsync(viewContext);

                return output.ToString();
            }
        }
    }
}
