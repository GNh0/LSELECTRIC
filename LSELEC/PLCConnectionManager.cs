using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using LSELEC.Communication;
using Timer = System.Timers.Timer;

namespace LSELEC
{
    /// <summary>
    /// PLC 연결 관리자 - 여러 PLC 연결을 관리하고 모니터링
    /// </summary>
    public class PLCConnectionManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, object> _connections;
        private readonly Timer _healthCheckTimer;
        private readonly int _healthCheckIntervalMs;
        private bool _disposed;

        /// <summary>
        /// 연결 상태 변경 이벤트
        /// </summary>
        public event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;

        public PLCConnectionManager(int healthCheckIntervalMs = 5000)
        {
            _connections = new ConcurrentDictionary<string, object>();
            _healthCheckIntervalMs = healthCheckIntervalMs;
            _healthCheckTimer = new Timer(_healthCheckIntervalMs);
            _healthCheckTimer.Elapsed += HealthCheckTimer_Elapsed;
            _healthCheckTimer.Start();
        }

        /// <summary>
        /// PLC 연결 추가
        /// </summary>
        public bool AddConnection(string connectionId, object plcClient)
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                throw new ArgumentException("연결 ID는 비어 있을 수 없습니다.", nameof(connectionId));

            if (plcClient == null)
                throw new ArgumentNullException(nameof(plcClient));

            return _connections.TryAdd(connectionId, plcClient);
        }

        /// <summary>
        /// PLC 연결 제거
        /// </summary>
        public bool RemoveConnection(string connectionId)
        {
            if (_connections.TryRemove(connectionId, out var client))
            {
                if (client is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// PLC 연결 가져오기
        /// </summary>
        public object? GetConnection(string connectionId)
        {
            _connections.TryGetValue(connectionId, out var client);
            return client;
        }

        /// <summary>
        /// 모든 연결 수 가져오기
        /// </summary>
        public int ConnectionCount => _connections.Count;

        private void HealthCheckTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            foreach (var kvp in _connections)
            {
                try
                {
                    // 연결 상태 확인 로직
                    // 실제 구현에서는 각 클라이언트의 연결 상태를 확인해야 함
                    bool isConnected = CheckConnection(kvp.Value);
                    
                    OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(
                        kvp.Key, isConnected));
                }
                catch (Exception ex)
                {
                    OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(
                        kvp.Key, false, ex.Message));
                }
            }
        }

        private bool CheckConnection(object client)
        {
            // 기본 구현: 항상 연결됨으로 간주
            // 실제로는 각 클라이언트 타입에 따라 연결 상태를 확인해야 함
            return client != null;
        }

        protected virtual void OnConnectionStatusChanged(ConnectionStatusChangedEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, e);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _healthCheckTimer?.Stop();
            _healthCheckTimer?.Dispose();

            foreach (var client in _connections.Values)
            {
                if (client is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            _connections.Clear();

            _disposed = true;
        }
    }

    /// <summary>
    /// 연결 상태 변경 이벤트 인자
    /// </summary>
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public string ConnectionId { get; }
        public bool IsConnected { get; }
        public string? ErrorMessage { get; }

        public ConnectionStatusChangedEventArgs(string connectionId, bool isConnected, string? errorMessage = null)
        {
            ConnectionId = connectionId;
            IsConnected = isConnected;
            ErrorMessage = errorMessage;
        }
    }
}
