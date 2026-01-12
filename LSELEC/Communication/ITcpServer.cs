using System;
using System.Net;
using System.Threading.Tasks;

namespace LSELEC.Communication
{
    /// <summary>
    /// TCP 서버 인터페이스
    /// </summary>
    public interface ITcpServer : IDisposable
    {
        /// <summary>
        /// 서버가 실행 중인지 여부
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// 서버 포트
        /// </summary>
        int Port { get; }

        /// <summary>
        /// 연결된 클라이언트 수
        /// </summary>
        int ConnectedClientCount { get; }

        /// <summary>
        /// 클라이언트 연결 이벤트
        /// </summary>
        event EventHandler<ClientConnectedEventArgs>? ClientConnected;

        /// <summary>
        /// 클라이언트 연결 해제 이벤트
        /// </summary>
        event EventHandler<ClientDisconnectedEventArgs>? ClientDisconnected;

        /// <summary>
        /// 데이터 수신 이벤트
        /// </summary>
        event EventHandler<DataReceivedEventArgs>? DataReceived;

        /// <summary>
        /// 서버 시작
        /// </summary>
        Task StartAsync();

        /// <summary>
        /// 서버 중지
        /// </summary>
        Task StopAsync();

        /// <summary>
        /// 클라이언트에게 데이터 전송
        /// </summary>
        Task SendAsync(string clientId, byte[] data);

        /// <summary>
        /// 모든 클라이언트에게 데이터 브로드캐스트
        /// </summary>
        Task BroadcastAsync(byte[] data);

        /// <summary>
        /// 클라이언트 연결 해제
        /// </summary>
        Task DisconnectClientAsync(string clientId);
    }

    /// <summary>
    /// 클라이언트 연결 이벤트 인자
    /// </summary>
    public class ClientConnectedEventArgs : EventArgs
    {
        public string ClientId { get; }
        public IPEndPoint RemoteEndPoint { get; }
        public DateTime ConnectedTime { get; }

        public ClientConnectedEventArgs(string clientId, IPEndPoint remoteEndPoint)
        {
            ClientId = clientId;
            RemoteEndPoint = remoteEndPoint;
            ConnectedTime = DateTime.Now;
        }
    }

    /// <summary>
    /// 클라이언트 연결 해제 이벤트 인자
    /// </summary>
    public class ClientDisconnectedEventArgs : EventArgs
    {
        public string ClientId { get; }
        public DateTime DisconnectedTime { get; }
        public string? Reason { get; }

        public ClientDisconnectedEventArgs(string clientId, string? reason = null)
        {
            ClientId = clientId;
            DisconnectedTime = DateTime.Now;
            Reason = reason;
        }
    }

    /// <summary>
    /// 데이터 수신 이벤트 인자
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        public string ClientId { get; }
        public byte[] Data { get; }
        public DateTime ReceivedTime { get; }

        public DataReceivedEventArgs(string clientId, byte[] data)
        {
            ClientId = clientId;
            Data = data;
            ReceivedTime = DateTime.Now;
        }
    }
}
