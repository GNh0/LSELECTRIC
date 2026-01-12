using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LSELEC.Communication
{
    /// <summary>
    /// LS XGT FEnet 클라이언트
    /// </summary>
    public class LSXGTFEnetClient : IDisposable
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly LSElecHeaderMaker.PLCCpuType _cpuType;
        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;
        private bool _disposed;
        private ushort _invokeId;
        private readonly byte _fenetPosition;

        /// <summary>
        /// 연결 상태
        /// </summary>
        public bool IsConnected => _tcpClient?.Connected ?? false;

        /// <summary>
        /// 연결 이벤트
        /// </summary>
        public event EventHandler? Connected;

        /// <summary>
        /// 연결 해제 이벤트
        /// </summary>
        public event EventHandler? Disconnected;

        public LSXGTFEnetClient(string ipAddress, int port, LSElecHeaderMaker.PLCCpuType cpuType, byte fenetPosition = 0)
        {
            _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            _port = port;
            _cpuType = cpuType;
            _fenetPosition = fenetPosition;
            _invokeId = 1;
        }

        /// <summary>
        /// PLC 연결
        /// </summary>
        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;

            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(_ipAddress, _port);
                _networkStream = _tcpClient.GetStream();
                
                OnConnected();
            }
            catch (Exception ex)
            {
                throw new Exception($"PLC 연결 실패: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// PLC 연결 해제
        /// </summary>
        public void Disconnect()
        {
            if (!IsConnected)
                return;

            try
            {
                _networkStream?.Close();
                _tcpClient?.Close();
                
                OnDisconnected();
            }
            catch (Exception ex)
            {
                throw new Exception($"PLC 연결 해제 실패: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 데이터 읽기
        /// </summary>
        public async Task<byte[]> ReadAsync(string deviceAddress, int length)
        {
            if (!IsConnected)
                throw new InvalidOperationException("PLC에 연결되지 않았습니다.");

            try
            {
                // FEnet 읽기 명령어 생성
                var instruction = FEnetInstruction.CreateReadInstruction(deviceAddress, length);
                var instructionData = instruction.ToBytes();

                // 헤더와 패킷 생성
                var packet = LSElecHeaderMaker.CreateClientPacket(_cpuType, instructionData, _fenetPosition, _invokeId++);

                // 데이터 전송
                await _networkStream!.WriteAsync(packet, 0, packet.Length);

                // 응답 수신
                var response = new byte[1024];
                var bytesRead = await _networkStream.ReadAsync(response, 0, response.Length);

                // 헤더 제거하고 데이터만 반환 (헤더는 20바이트)
                var dataLength = bytesRead - 20;
                var result = new byte[dataLength];
                Array.Copy(response, 20, result, 0, dataLength);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"데이터 읽기 실패: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 데이터 쓰기
        /// </summary>
        public async Task WriteAsync(string deviceAddress, byte[] data)
        {
            if (!IsConnected)
                throw new InvalidOperationException("PLC에 연결되지 않았습니다.");

            try
            {
                // FEnet 쓰기 명령어 생성
                var instruction = FEnetInstruction.CreateWriteInstruction(deviceAddress, data);
                var instructionData = instruction.ToBytes();

                // 헤더와 패킷 생성
                var packet = LSElecHeaderMaker.CreateClientPacket(_cpuType, instructionData, _fenetPosition, _invokeId++);

                // 데이터 전송
                await _networkStream!.WriteAsync(packet, 0, packet.Length);

                // 응답 수신 (쓰기 응답 확인)
                var response = new byte[256];
                await _networkStream.ReadAsync(response, 0, response.Length);
            }
            catch (Exception ex)
            {
                throw new Exception($"데이터 쓰기 실패: {ex.Message}", ex);
            }
        }

        protected virtual void OnConnected()
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Disconnect();
            _networkStream?.Dispose();
            _tcpClient?.Dispose();

            _disposed = true;
        }
    }
}
