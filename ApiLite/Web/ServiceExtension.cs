using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiLite.Web;

static class ServiceExtension
{
    internal static WebApplicationBuilder AddKApp(this WebApplicationBuilder builder, Action<AppOption>? action = null)
    {
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        var option = new AppOption();
        action?.Invoke(option);

        var mvcBuilder = builder.Services.AddControllers(o =>
        {
            o.EnableEndpointRouting = false;
            option.MvcOption?.Invoke(o);
        });

        mvcBuilder.AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.PropertyNamingPolicy = null;
            option.JsonOption?.Invoke(o);
        });

        AddDynamicApi(mvcBuilder, option);

        builder.Services.AddSession();

        return builder;
    }

    private static void AddDynamicApi(IMvcBuilder builder, AppOption option)
    {
        builder.ConfigureApplicationPartManager(m =>
        {
            m.ApplicationParts.Add(new AssemblyPart(typeof(IService).Assembly));
            foreach (var item in option.Modules)
            {
                item.Initialize();
                var assembly = item.GetType().Assembly;
                m.ApplicationParts.Add(new AssemblyPart(assembly));
            }
            m.FeatureProviders.Add(new ApiFeatureProvider());
        });

        builder.Services.Configure<MvcOptions>(o =>
        {
            o.Conventions.Add(new ApiConvention());
        });
    }

    internal static WebApplication UseKApp(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/?m=Error500");
        }

        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseSession();
        app.UseAuthorization();
        app.UseWebSockets();
        app.UseMvc();
        app.MapControllers();

        return app;
    }
}
