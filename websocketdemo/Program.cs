using System.Net.WebSockets;
using System.Text;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseWebSockets(); //Enable websocket
var connections=new List<WebSocket>(); //List store websocket for all clients

//Define enpoint for client contect to
app.Map("/ws", async context=>{
    //Check normal request or websocket request
    if(context.WebSockets.IsWebSocketRequest) 
    {
        //Create websocket connection for client
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        //Add just created websocket to list
        connections.Add(ws);

        String message = "Xin chao thang client!";
        
        var bytes = Encoding.UTF8.GetBytes(message);
        if(ws.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

            var buffer = new byte[1024*4];
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            String strReceived = Encoding.UTF8.GetString(buffer);
        }        
    }
});

app.UseHttpsRedirection();
app.Run();