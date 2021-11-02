using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApiLite.Web;

public class KHost
{
    public static void Run(string[] args, Action<AppOption>? action = null)
    {
        var app = Configure(args, action);
        app.Run();
    }

    public static Task RunAsync(string[] args, Action<AppOption>? action = null)
    {
        var app = Configure(args, action);
        return app.RunAsync();
    }

    private static WebApplication Configure(string[] args, Action<AppOption>? action = null)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddKApp(action);
        builder.Services.AddRazorPages();

        var app = builder.Build();
        app.UseKApp();
        app.MapRazorPages();
        return app;
    }
}
