using Microsoft.AspNetCore.Mvc;
using WxConnectorLib.Managers;
using WxConnectorLib.Utils;
using WxConnectorProvider.Models;
using WxConnectorProvider.Providers;

namespace WxConnectorProvider.Controllers;

/// <summary>
/// 这个 ApiController 用于管理 Wx生命周期 相关的 Api
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WxController
{

    [HttpPost("startWx")]
    public IResult StartWxAction([FromBody] StartWxActionModel data)
    {
        if (data.WxPath == string.Empty) return Results.Json(
            new {message = "WxPath 不能为空"},
            statusCode: 400
            );
        ActionUtil.Get().LoginAction(data.WxPath);
        return Results.Json(
            new { message = "Wx 启动成功" },
            statusCode: 200
        );
    }

    [HttpPost("listen")]
    public IResult ListenAction([FromBody] ListenActionModel data)
    {
        if (data.Listeners.Count == 0) return Results.Json(
            new { message = "Listeners 不能为空" },
            statusCode: 400
        );
        var listeners = ActionUtil.Get().OpenListenerWindowsAction(data.Listeners);
        DataProvider.Get().ListeningWindows.AddRange(listeners);
        ListenManager.Get().InitListen(listeners);
        return Results.Json(
            new { message = "监听器启动成功" },
            statusCode: 200
        );
    }
}
