namespace ApiLite;

[AttributeUsage(AttributeTargets.Method)]
public class RouteAttribute : Attribute
{
    public RouteAttribute(string path)
    {
        Path = path;
    }

    public string Path { get; }
}
