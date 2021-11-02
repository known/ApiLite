using Microsoft.AspNetCore.Mvc;

namespace ApiLite.Web;

public class AppOption
{
    public AppOption()
    {
        Modules = new List<IModule>();
    }

    public List<IModule> Modules { get; }
    public Action<MvcOptions> MvcOption { get; set; }
    public Action<JsonOptions> JsonOption { get; set; }
}
