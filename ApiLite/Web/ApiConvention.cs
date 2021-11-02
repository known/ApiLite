using System.Text;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ApiLite.Web;

class ApiConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var type = controller.ControllerType;
            if (typeof(IService).IsAssignableFrom(type))
            {
                ConfigureApiExplorer(controller);
                ConfigureSelector(controller);
            }
        }
    }

    private static void ConfigureApiExplorer(ControllerModel controller)
    {
        if (!controller.ApiExplorer.IsVisible.HasValue)
            controller.ApiExplorer.IsVisible = true;

        foreach (var action in controller.Actions)
        {
            if (!action.ApiExplorer.IsVisible.HasValue)
            {
                action.ApiExplorer.IsVisible = true;
            }
        }
    }

    private void ConfigureSelector(ControllerModel controller)
    {
        RemoveEmptySelectors(controller.Selectors);

        if (controller.Selectors.Any(selector => selector.AttributeRouteModel != null))
            return;

        foreach (var action in controller.Actions)
        {
            ConfigureSelector(action);
        }
    }

    private static void RemoveEmptySelectors(IList<SelectorModel> selectors)
    {
        for (var i = selectors.Count - 1; i >= 0; i--)
        {
            var selector = selectors[i];
            if (selector.AttributeRouteModel == null &&
               (selector.ActionConstraints == null || selector.ActionConstraints.Count <= 0) &&
               (selector.EndpointMetadata == null || selector.EndpointMetadata.Count <= 0))
            {
                selectors.Remove(selector);
            }
        }
    }

    private void ConfigureSelector(ActionModel action)
    {
        RemoveEmptySelectors(action.Selectors);

        if (action.Selectors.Count <= 0)
            AddServiceSelector(action);
        else
            NormalizeSelectorRoutes(action);
    }

    private void AddServiceSelector(ActionModel action)
    {
        var template = new Microsoft.AspNetCore.Mvc.RouteAttribute(GetRouteTemplate(action));
        var selector = new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(template)
        };
        selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));
        action.Selectors.Add(selector);
    }

    private void NormalizeSelectorRoutes(ActionModel action)
    {
        foreach (var selector in action.Selectors)
        {
            if (selector.AttributeRouteModel == null)
            {
                var template = new Microsoft.AspNetCore.Mvc.RouteAttribute(GetRouteTemplate(action));
                selector.AttributeRouteModel = new AttributeRouteModel(template);
            }

            if (selector.ActionConstraints.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods?.FirstOrDefault() == null)
                selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));
        }
    }

    private string GetRouteTemplate(ActionModel action)
    {
        if (action.Attributes != null && action.Attributes.Count > 0)
        {
            foreach (var item in action.Attributes)
            {
                if (item is RouteAttribute attribute)
                {
                    return attribute.Path;
                }
            }
        }

        var routeTemplate = new StringBuilder();
        //routeTemplate.Append("api");
        var names = action.Controller.ControllerType.Namespace.Split('.');
        if (names.Length > 2)
        {
            routeTemplate.Append(names[^2]);
        }

        // Controller
        var controllerName = action.Controller.ControllerName;
        if (controllerName.EndsWith("Service"))
            controllerName = controllerName[0..^7];

        routeTemplate.Append($"/{controllerName}");

        // Action
        var actionName = action.ActionName;
        if (actionName.EndsWith("Async"))
            actionName = actionName[..^"Async".Length];

        if (!string.IsNullOrEmpty(actionName))
            routeTemplate.Append($"/{actionName}");

        return routeTemplate.ToString();
    }

    private static string GetHttpMethod(ActionModel action)
    {
        var actionName = action.ActionName;
        if (actionName.StartsWith("Get"))
            return "GET";

        return "POST";
    }
}
