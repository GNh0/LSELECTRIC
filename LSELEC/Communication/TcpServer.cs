using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace LSELEC.Communication
{
    /// <summary>
    /// TCP 서버 구현
    /// </summary>
    public class TcpServer : ITcpServer
    {
        private readonly int _port;
        private TcpListener? _listener;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly ConcurrentDictionary<string, TcpClient> _clients;
        private bool _disposed;

        public bool IsRunning { get; private set; }
        public int Port => _port;
        public int ConnectedClientCount => _clients.Count;

        public event EventHandler<ClientConnectedEventArgs>? ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs>? ClientDisconnected;
        public event EventHandler<DataReceivedEventArgs>? DataReceived;

        public TcpServer(int port)
        {
            _port = port;
            _clients = new ConcurrentDictionary<string, TcpClient>();
        }

        public async Task StartAsync()
        {
            if (IsRunning)
                return;

            try
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();
                _cancellationTokenSource = new CancellationTokenSource();
                IsRunning = true;

                // 클라이언트 연결 대기
                _ = Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"서버 시작 실패: {ex.Message}", ex);
            }
        }

        public async Task StopAsync()
        {
            if (!IsRunning)
                return;

            try
            {
                IsRunning = false;
                _cancellationTokenSource?.Cancel();
                _listener?.Stop();

                // 모든 클라이언트 연결 해제
                foreach (var client in _clients.Values)
                {
                    client.Close();
                }
                _clients.Clear();

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"서버 중지 실패: {ex.Message}", ex);
            }
        }

        public async Task SendAsync(string clientId, byte[] data)
        {
            if (!_clients.TryGetValue(clientId, out var client))
                throw new InvalidOperationException($"클라이언트를 찾을 수 없습니다: {clientId}");

            try
            {
                var stream = client.GetStream();
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                throw new Exception($"데이터 전송 실패: {ex.Message}", ex);
            }
        }

        public async Task BroadcastAsync(byte[] data)
        {
            foreach (var kvp in _clients)
            {
                try
                {
                    await SendAsync(kvp.Key, data);
                }
                catch
                {
                    // 실패한 클라이언트는 무시
                }
            }
        }

        public async Task DisconnectClientAsync(string clientId)
        {
            if (_clients.TryRemove(clientId, out var client))
            {
                try
                {
                    client.Close();
                    OnClientDisconnected(new ClientDisconnectedEventArgs(clientId, "서버에서 연결 해제"));
                }
                catch (Exception ex)
                {
                    throw new Exception($"클라이언트 연결 해제 실패: {ex.Message}", ex);
                }
            }

            await Task.CompletedTask;
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && IsRunning)
            {
                try
                {
                    var client = await _listener!.AcceptTcpClientAsync();
                    var clientId = Guid.NewGuid().ToString();
                    
                    if (_clients.TryAdd(clientId, client))
                    {
                        var endPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
                        OnClientConnected(new ClientConnectedEventArgs(clientId, endPoint));

                        // 클라이언트 데이터 수신 시작
                        _ = Task.Run(() => ReceiveDataAsync(clientId, client, cancellationToken));
                    }
                }
                catch (Exception)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // 오류 처리 (로깅 등)
                    }
                }
            }
        }

        private async Task ReceiveDataAsync(string clientId, TcpClient client, CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            try
            {
                var stream = client.GetStream();

                while (!cancellationToken.IsCancellationRequested && client.Connected)
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                    if (bytesRead == 0)
                    {
                        // 클라이언트 연결 해제
                        break;
                    }

                    var data = new byte[bytesRead];
                    Array.Copy(buffer, 0, data, 0, bytesRead);

                    OnDataReceived(new DataReceivedEventArgs(clientId, data));
                }
            }
            catch (Exception)
            {
                // 오류 처리
            }
            finally
            {
                // 클라이언트 제거
                if (_clients.TryRemove(clientId, out _))
                {
                    client.Close();
                    OnClientDisconnected(new ClientDisconnectedEventArgs(clientId, "클라이언트 연결 해제"));
                }
            }
        }

        protected virtual void OnClientConnected(ClientConnectedEventArgs e)
        {
            ClientConnected?.Invoke(this, e);
        }

        protected virtual void OnClientDisconnected(ClientDisconnectedEventArgs e)
        {
            ClientDisconnected?.Invoke(this, e);
        }

        protected virtual void OnDataReceived(DataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            StopAsync().Wait();
            _cancellationTokenSource?.Dispose();

            _disposed = true;
        }
    }
}
