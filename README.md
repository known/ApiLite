# ApiLite
ApiLite是直接将Service层自动生成api路由，可以不用添加Controller，支持模块插件化，在项目开发中能够提高工作效率，降低代码量。
##开发环境
- .NET SDK 6.0.100-rc.2.21505.57
- VS2022 Preview 7.0
##示例目标
- 根据Service动态生成api
- 支持自定义路由模板（通过Route特性定义）
- 支持模块插件化
- 支持不同模块，相同Service名称的路由（命名空间需要有3级以上，例如：Com.Mod.XXX）
- 自动根据方法名称判断请求方式，Get开头的方法名为GET请求，其他为POST请求
##编码约定
- 模块类库必须包含继承IModule接口的类
- 需要生成api的Service必须继承IService接口
- GET请求的方法必须以Get开头
##使用示例
``` C#
KHost.Run(args, o =>
{
    o.Modules.Add(new TestModule());//添加模块
});

class TestModule : IModule
{
    public void Initialize()
    {
    }
}

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
```