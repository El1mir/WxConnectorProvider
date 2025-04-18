using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WxConnectorLib.Managers;
using WxConnectorLib.Utils;
using WxConnectorProvider.Models;
using WxConnectorProvider.Providers;

namespace WxConnectorProvider.Controllers;

/// <summary>
/// 这个方法用于处理操作有关的请求
/// </summary>
[ApiController]
[Route("/api/[controller]")]
public class ActionController
{
    /// <summary>
    /// 用于发送普通文本消息
    /// </summary>
    /// <param name="data">发送文本消息的请求体</param>
    /// <returns>Http返回</returns>
    [HttpPost]
    [Route("sendText")]
    public IResult SendTextAction([FromBody] SendTextActionModel data)
    {
        ActionUtil.Get().SendTextMessage(
            data.Msg,
            DataProvider.Get().ListeningWindows.First(x => x.Title == data.SendWindowTitle)
        );
        return Results.Json(
            new { message = "发送成功" },
            statusCode: StatusCodes.Status200OK
        );
    }

    [HttpPost]
    [Route("sendFile")]
    public IResult SendFileAction([FromBody] SendFileActionModel data)
    {
        ActionUtil.Get().SendFileMessage(
            data.FilePath,
            DataProvider.Get().ListeningWindows.First(x => x.Title == data.SendWindowTitle)
        );
        return Results.Json(
            new { message = "发送成功" },
            statusCode: StatusCodes.Status200OK
        );
    }

    [HttpPost]
    [Route("AtByUserNameInGroup")]
    public IResult AtByUserNameInGroupAction([FromBody] AtByUserNameInGroupActionModel data)
    {
        ActionUtil.Get().AtByNameInGroup(
            data.UserName,
            DataProvider.Get().ListeningWindows.First(x => x.Title == data.WindowTitle)
        );
        return Results.Json(
            new { message = "AT成功" },
            statusCode: StatusCodes.Status200OK
        );
    }
}
