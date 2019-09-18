using System.Threading.Tasks;

namespace RazorRender
{
    public interface IRazorRenderService
    {
        Task<string> ViewToStringAsync<T>(string viewName, T model);
        Task<string> ViewToStringAsync(string viewName);
    }
}
