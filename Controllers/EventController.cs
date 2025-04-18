using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WxConnectorLib.Managers;

namespace WxConnectorProvider.Controllers;

/// <summary>
/// 这个类用于管理 WebSocket 连接
/// 用于进行 Event 事件的推送
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    public EventController()
    {
        InitEventBind();
    }

    private static readonly List<WebSocket> Sockets = [];

    /// <summary>
    /// 这个方法托管了 WS 端点
    /// </summary>
    [HttpGet("ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            Sockets.Add(webSocket);

            await HandleWebSocket(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status423Locked;
        }
    }

    /// <summary>
    /// 用于处理 WebSocket 连接（保持连接）
    /// </summary>
    /// <param name="webSocket">DI 注入 WS 对象</param>
    private async Task HandleWebSocket(WebSocket webSocket)
    {
        var buffer = new ArraySegment<byte>(new byte[8192]);
        WebSocketReceiveResult receiveResult;
        do
        {
            receiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            if (receiveResult.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        } while (!receiveResult.CloseStatus.HasValue);

        Sockets.Remove(webSocket);
    }

    /// <summary>
    /// 这个方法用于向所有连接的 WebSocket 客户端发送消息
    /// </summary>
    /// <param name="data">需要发送的 data 对象</param>
    private async Task SendToAllAsync(object data)
    {
        var buffer = new ArraySegment<byte>(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data))
            );

        foreach (var socket in Sockets.ToArray())
        {
            try
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(
                        buffer,
                        WebSocketMessageType.Text,
                        endOfMessage: true,
                        CancellationToken.None
                    );
                }
                else
                {
                    Sockets.Remove(socket);
                }
            }
            catch (WebSocketException)
            {
                Sockets.Remove(socket);
            }
        }
    }

    /// <summary>
    /// 用于初始化事件绑定
    /// </summary>
    private void InitEventBind()
    {
        EventManager.Get().OnNewMessageWithoutSelf += (msg, _) =>
        {
            Task.Run(async () =>
            {
                await SendToAllAsync(new
                {
                    eventType = "NewMessageWithoutSelfEvent",
                    data = msg
                });
            });
        };
        EventManager.Get().OnNewMessage += (msg, _) =>
        {
            Task.Run(async () =>
            {
                await SendToAllAsync(new
                {
                    eventType = "NewMessageEvent",
                    data = msg
                });
            });
        };
    }
}
