using System;
using LSELEC.Communication;

namespace LSELEC
{
    /// <summary>
    /// LS PLC 클라이언트 팩토리 (.NET Framework 4.7.2 호환)
    /// Cnet 및 FEnet 자동 선택
    /// </summary>
    public static class PLCClientFactory
    {
        /// <summary>
        /// PLC 통신 유형
        /// </summary>
        public enum CommunicationType
        {
            /// <summary>
            /// Cnet 통신 (직렬 통신 기반)
            /// </summary>
            Cnet,
            
            /// <summary>
            /// FEnet 통신 (이더넷 기반)
            /// </summary>
            FEnet
        }

        /// <summary>
        /// 지정된 통신 유형에 따라 PLC 클라이언트 생성
        /// </summary>
        /// <param name="type">통신 유형</param>
        /// <param name="ipAddress">PLC IP 주소</param>
        /// <param name="port">PLC 포트 번호</param>
        /// <param name="cpuType">CPU 타입</param>
        /// <returns>생성된 PLC 클라이언트</returns>
        public static object CreateClient(CommunicationType type, string ipAddress, int port, LSElecHeaderMaker.PLCCpuType cpuType)
        {
            switch (type)
            {
                case CommunicationType.FEnet:
                    return new LSXGTFEnetClient(ipAddress, port, cpuType);
                
                case CommunicationType.Cnet:
                    return new LSXGTPLCClient(ipAddress, port, cpuType);
                
                default:
                    throw new ArgumentException($"지원되지 않는 통신 유형입니다: {type}");
            }
        }

        /// <summary>
        /// 자동으로 통신 유형을 감지하여 PLC 클라이언트 생성
        /// </summary>
        /// <param name="ipAddress">PLC IP 주소</param>
        /// <param name="port">PLC 포트 번호</param>
        /// <param name="cpuType">CPU 타입</param>
        /// <returns>생성된 PLC 클라이언트</returns>
        public static object CreateClientAuto(string ipAddress, int port, LSElecHeaderMaker.PLCCpuType cpuType)
        {
            // 기본적으로 FEnet을 사용 (더 현대적인 통신 방식)
            return CreateClient(CommunicationType.FEnet, ipAddress, port, cpuType);
        }
    }
}
