using System;
using System.Net.Sockets;

namespace Server.Network.WebSocket;

public class WebSocket
{
    private static readonly byte[] WebSocketCannedResponse =
        "HTTP/1.1 101 Switching Protocols\r\nConnection: Upgrade\r\nUpgrade: websocket\r\nSec-WebSocket-Accept: hmJpHzvXcd3qiksRp/bsqj4e2X8=\r\n\r\n"u8
            .ToArray();

    public static void Initialize()
    {
        EventSink.SocketConnect += OnEventSinkOnSocketConnect;
    }

    private static void OnEventSinkOnSocketConnect(SocketConnectEventArgs args)
    {
        var socket = args.Connection;
        var peek = new byte[3];
        socket.Receive(peek, SocketFlags.Peek);

        if (!peek.AsSpan().SequenceEqual("GET"u8))
        {
            return;
        }

        var buffer = new byte[1024];
        socket.Receive(buffer);
        socket.Send(WebSocketCannedResponse);
    }
}
