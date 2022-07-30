using LeeFramework.Web;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    private WebSocketNetwork _WebSocket;

    private Dictionary<string, WebSocketNetwork> _AllWebClient = new Dictionary<string, WebSocketNetwork>();

    private void Start()
    {
        _WebSocket = new WebSocketNetwork();

        _WebSocket.onConnect = (state) =>
          {
              Debug.Log("Connect : " + state.ToString());
          };

        _WebSocket.onReceive = (state, data, res) =>
        {
            if (!_IsSend)
            {
                return;
            }

            _Index++;

            Debug.Log("接收 ： " + Encoding.UTF8.GetString(data) + "  Index : " + _Index);

            Debug.Log("  Index : " + _Index);

            _WebSocket.SendAsync(Encoding.UTF8.GetBytes("客户端哈哈"));
        };

        _WebSocket.onSend = (state) =>
        {
            Debug.Log("发送 ： " + state.ToString());
        };

        _WebSocket.onClose = (state, socketState) =>
        {
            Debug.Log("Close : " + state.ToString() + "  " + socketState.ToString());
        };

        _AllWebClient.Add("1", _WebSocket);
    }

    bool _IsSend = false;
    float _Timer = 0;
    float _Time = 1;
    int _Index = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _IsSend = true;
            _Timer = 0;
            _Index = 0;
            _WebSocket.SendAsync(Encoding.UTF8.GetBytes("Hello"));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _WebSocket.CloseAsync();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _WebSocket.ConnectAsync("ws://127.0.0.1:1500");
        }
        if (_IsSend)
        {
            _Timer += Time.deltaTime;
            if (_Timer > _Time)
            {
                _IsSend = false;
            }
            _WebSocket.SendAsync(Encoding.UTF8.GetBytes("Hello"));
        }

    }
}

