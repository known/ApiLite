namespace ApiLite.Web;

public class TestService : IService
{
    public string GetName(string name)
    {
        return $"Hello {name}";
    }

    public string SaveData(string data)
    {
        return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {data}";
    }

    [Route("api/test")]
    public string GetCustMethod(string id)
    {
        return id;
    }
}
