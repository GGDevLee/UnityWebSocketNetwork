# WebSocketNetwork

**联系作者：419731519（QQ）**

### =================WebSocketNetwork介绍=================
#### WebSocket是一种在单个Tcp连接进行全双工通讯的协议，也使得客户端和服务器之间的数据交互更加简单
#### 允许服务器向客户端主动推送数据，两者只需握手一次，即可创建持续性的连接，并进行双向数据传输
#### 笔者的这个插件，完全封装了微软提供的System.Net.WebSockets，只需要简单的处理，就可以进行网络通讯，非常方便

### =================关于异步接收消息=================
#### WebSocketNetwork.onConnect连接状态成功后，会立马开始接收消息
#### 所以开发者，只需要监听WebSocketNetwork.onReceive即可接收消息
#### 一般情况下，网络消息是在多线程下接收的，而多线程的消息，不能直接给Unity使用
#### 笔者使用了MainThread，使得多线程的消息，安全转移到Unity主线程中
#### 所以开发者可以安全的使用WebSocketNetwork.onReceive返回的消息，不需要再次转移到Unity主线程中

### =================关于异步发送消息=================
#### 笔者使用了消息队列的方式来处理异步发送的消息，即上一条消息未发送完成，就不会发送下一条消息
#### 以确保服务器所收到的消息，是有序

### =================使用方法=================
- manifest.json中添加插件路径 或 直接引用Release内的dll（二选一）
```json
{
  "dependencies": {
	"com.leeframework.websocketnetwork":"https://e.coding.net/ggdevlee/leeframework/WebSocketNetwork.git#1.0.0"
  }
}
```

- 引入命名空间
```csharp
using LeeFramework.Web;
```

- TWebSocketNetwork初始化
```csharp
private WebSocketNetwork _WebSocket;

private void Init()
{
    _WebSocket = new WebSocketNetwork();

    _WebSocket.onConnect = (state) =>
    {
        Debug.Log("Connect : " + state.ToString());
    };

    _WebSocket.onReceive = (state, data, res) =>
    {
        Debug.Log("Receive ： " + Encoding.UTF8.GetString(data));
    };
    
    _WebSocket.onSend = (state) =>
    {
        Debug.Log("Send ： " + state.ToString());
    };

    _WebSocket.onClose = (state, socketState) =>
    {
        Debug.Log("Close : " + state.ToString() + "  " + socketState.ToString());
    };
}        
```

- 异步创建网络连接
- 异步连接成功与失败，都会触发到WebSocketNetwork.onConnect事件
- 异步连接成功后，会自动接收消息，接收到的消息，都会触发WebSocketNetwork.onReceive事件
```csharp
private WebSocketNetwork _WebSocket;

public void ConnectAsync()
{
    _WebSocket.ConnectAsync("ws://127.0.0.1:1500");
}
```

- 异步发送消息
- 异步发送成功与失败，都会触发到WebSocketNetwork.onSend事件
```csharp

public void SendAsync()
{
    //会经过消息队列，上一条发送完，这条才会发送
    _WebSocket.SendAsync(Encoding.UTF8.GetBytes("HelloWorld!"));

    //立马发送消息
    _WebSocket.Send(Encoding.UTF8.GetBytes("HelloWorld!"));
}

```

- 异步断开连接
- 异步断开连接成功与失败，都会触发到WebSocketNetwork.onClose事件

```csharp

public void CloseAsync()
{
    _WebSocket.CloseAsync();
}

```

- WebSocketNetwork网络状态
```csharp

_WebSocket.webSocket //ClientWebSocket

_WebSocket.isConnecting //正在网络连接

_WebSocket.isConnected //是否已经网络连接

_WebSocket.isClosing //正在断开连接

_WebSocket.isClosed //是否已经断开连接

_WebSocket.isSending //正在发送消息

```
        
