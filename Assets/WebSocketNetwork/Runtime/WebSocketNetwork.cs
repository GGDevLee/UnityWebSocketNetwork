using System.Net.WebSockets;
using System.Threading;
using System;
using System.Text;

namespace LeeFramework.Web
{
    public class WebSocketNetwork : WebSocketBase
    {
        /// <summary>
        /// ��ʼ�첽����
        /// </summary>
        public async void ConnectAsync(string url)
        {
            _WebSocket = new ClientWebSocket();

            await _WebSocket.ConnectAsync(new Uri(url), CancellationToken.None);

            if (_WebSocket.State == WebSocketState.Open)
            {
                _OnConnect?.Invoke(true);

                //��ʼ����
                ReceiveAsync();
            }
            else
            {
                _OnConnect?.Invoke(false);
            }
        }

        /// <summary>
        /// �첽������Ϣ
        /// </summary>
        private async void ReceiveAsync()
        {
            if (_WebSocket == null || !isConnected)
            {
                return;
            }

            WebSocketReceiveResult result = await _WebSocket.ReceiveAsync(_Buff, CancellationToken.None);

            //����������Ϣ��
            if (result.EndOfMessage)
            {
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _OnReceive?.Invoke(false, _Buff, result);
                    return;
                }

                if (result.Count > ushort.MaxValue)
                {
                    CloseAsync(WebSocketCloseStatus.MessageTooBig);
                    return;
                }

                _OnReceive?.Invoke(true, _Buff, result);

                ReceiveAsync();
            }
        }

        /// <summary>
        /// �첽������Ϣ
        /// </summary>
        public async void SendAsync(byte[] data)
        {
            if (_WebSocket == null)
            {
                return;
            }

            if (_WebSocket.State != WebSocketState.Open)
            {
                return;
            }

            if (_IsSending)
            {
                SendItem item = _SendPool.Spawn();
                item.SetData(data);
                _SendQue.Enqueue(item);
                return;
            }

            _IsSending = true;

            await _WebSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);

            _IsSending = false;

            _OnSend?.Invoke(_WebSocket.State != WebSocketState.CloseSent);

            CheckSend();
        }

        /// <summary>
        /// ��������Ϣ���У�ֱ���첽����
        /// </summary>
        public void Send(byte[] data)
        {
            if (_WebSocket == null)
            {
                return;
            }

            if (_WebSocket.State != WebSocketState.Open)
            {
                return;
            }

            _WebSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        /// <summary>
        /// �첽������Ϣ
        /// </summary>
        public async void SendAsync(string content)
        {
            if (_WebSocket == null)
            {
                return;
            }

            if (_WebSocket.State != WebSocketState.Open)
            {
                return;
            }

            if (_IsSending)
            {
                SendItem item = _SendPool.Spawn();
                item.SetContent(content);
                _SendQue.Enqueue(item);
                return;
            }

            _IsSending = true;

            await _WebSocket.SendAsync(Encoding.UTF8.GetBytes(content), WebSocketMessageType.Text, true, CancellationToken.None);

            _IsSending = false;

            _OnSend?.Invoke(_WebSocket.State != WebSocketState.CloseSent);

            CheckSend();
        }

        /// <summary>
        /// ��������Ϣ���У�ֱ���첽����
        /// </summary>
        public void Send(string content)
        {
            if (_WebSocket == null)
            {
                return;
            }

            if (_WebSocket.State != WebSocketState.Open)
            {
                return;
            }

            _WebSocket.SendAsync(Encoding.UTF8.GetBytes(content), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void CheckSend()
        {
            if (_SendQue.Count > 0)
            {
                SendItem item = _SendQue.Dequeue();
                if (item != null)
                {
                    switch (item.type)
                    {
                        case WebSocketMessageType.Binary:
                            SendAsync(item.buff);
                            break;
                        case WebSocketMessageType.Text:
                            SendAsync(item.txt);
                            break;
                    }
                    _SendPool.Recycle(item);
                }
            }
        }

        /// <summary>
        /// �첽�Ͽ�����
        /// </summary>
        public async void CloseAsync(WebSocketCloseStatus status = WebSocketCloseStatus.NormalClosure)
        {
            if (_WebSocket == null)
            {
                return;
            }

            if (_WebSocket.State == WebSocketState.Open)
            {
                _IsClosing = true;

                await _WebSocket.CloseAsync(status, status.ToString(), CancellationToken.None);

                _IsClosing = false;

                _WebSocket.Abort();

                if (_WebSocket.State == WebSocketState.Closed)
                {
                    _OnClose?.Invoke(true, _WebSocket.State);
                }
                else
                {
                    _OnClose?.Invoke(false, _WebSocket.State);
                }
            }
            else
            {
                _OnClose?.Invoke(false, _WebSocket.State);
            }
        }

    }

}