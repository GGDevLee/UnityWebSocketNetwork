using System.Net.WebSockets;

public class SendItem
{
    public string txt = string.Empty;
    public byte[] buff = null;
    public WebSocketMessageType type;

    public void SetData(byte[] data)
    {
        buff = data;
        type = WebSocketMessageType.Binary;
    }

    public void SetContent(string content)
    {
        txt = content;
        type = WebSocketMessageType.Text;
    }

    public void Dispose()
    {
        txt = string.Empty;
        buff = null;
    }

}

