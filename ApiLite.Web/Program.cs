using ApiLite.Web;

KHost.Run(args, o =>
{
    o.Modules.Add(new TestModule());
});