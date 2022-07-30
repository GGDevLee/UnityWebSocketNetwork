using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace LeeFramework.Web
{
    public class WebSocketBase : IDisposable
    {
        public Action<bool> onConnect;
        public Action<bool, byte[], WebSocketReceiveResult> onReceive;
        public Action<bool> onSend;
        public Action<bool, WebSocketState> onClose;


        protected Action<bool> _OnConnect;
        protected Action<bool, byte[], WebSocketReceiveResult> _OnReceive;
        protected Action<bool> _OnSend;
        protected Action<bool, WebSocketState> _OnClose;

        public WebSocketBase()
        {
            _OnConnect = (state) =>
            {
                MainThread.instance.Run(() =>
                {
                    onConnect?.Invoke(state);
                });
            };

            _OnReceive = (state, buff, result) =>
            {
                MainThread.instance.Run(() =>
                {
                    onReceive?.Invoke(state, buff, result);
                });
            };

            _OnSend = (state) =>
            {
                MainThread.instance.Run(() =>
                {
                    onSend?.Invoke(state);
                });
            };

            _OnClose = (state, webState) =>
            {
                MainThread.instance.Run(() =>
                {
                    onClose?.Invoke(state, webState);
                });
            };
        }


        public ClientWebSocket webSocket => _WebSocket;
        public bool isConnecting
        {
            get
            {
                if (_WebSocket != null)
                {
                    return _WebSocket.State == WebSocketState.Connecting;
                }
                return false;
            }
        }
        public bool isConnected
        {
            get
            {
                if (_WebSocket != null)
                {
                    return _WebSocket.State == WebSocketState.Open;
                }
                return false;
            }
        }
        public bool isClosed
        {
            get
            {
                if (_WebSocket != null)
                {
                    return _WebSocket.State == WebSocketState.Closed;
                }
                return false;
            }
        }
        public bool isClosing => _IsClosing;
        public bool isSending => _IsSending;

        
        protected bool _IsClosing = false;
        protected bool _IsSending = false;

        protected Queue<SendItem> _SendQue = new Queue<SendItem>();
        protected SendItemPool _SendPool = new SendItemPool(20);

        protected ClientWebSocket _WebSocket;
        protected byte[] _Buff = new byte[ushort.MaxValue];

        public void Dispose()
        {

        }
    }
}