namespace LSELEC
{
    public class LSElecHeaderMaker
    {
        // 헤더 필드 인덱스 정의
        private const int IDX_COMPANY_ID_START = 0;      // 8바이트
        private const int IDX_RESERVED1_START = 8;       // 2바이트
        private const int IDX_PLC_INFO_START = 10;       // 2바이트
        private const int IDX_CPU_INFO = 12;             // 1바이트
        private const int IDX_SOURCE_OF_FRAME = 13;      // 1바이트
        private const int IDX_INVOKE_ID_START = 14;      // 2바이트
        private const int IDX_LENGTH_START = 16;         // 2바이트
        private const int IDX_FENET_POSITION = 18;       // 1바이트
        private const int IDX_BCC = 19;                  // 1바이트
        private const int HEADER_SIZE = 20;

        // CPU 타입 정의
        public enum PLCCpuType
        {
            XGK = 0xA0,
            XGI = 0xA4,
            XGR = 0xA8,
        }

        public enum CommunicationMode
        {
            Client = 0x33,
            Server = 0x11
        }

        private static byte[] MakeHeader(int length, ushort invokeId, ushort plcInfo, byte fenetPosition = 0, PLCCpuType cpuType = PLCCpuType.XGK, CommunicationMode mode = CommunicationMode.Client)
        {
            try
            {
                // 헤더 배열 생성
                byte[] header = new byte[HEADER_SIZE];

                // 1. 회사 ID (8바이트): "LSIS-XGT" 고정값
                byte[] companyIdBytes = System.Text.Encoding.ASCII.GetBytes("LSIS-XGT");
                Array.Copy(companyIdBytes, 0, header, IDX_COMPANY_ID_START, Math.Min(companyIdBytes.Length, 8));

                // 2. 예약 영역 (2바이트): 0x00 고정
                header[IDX_RESERVED1_START] = 0x00;
                header[IDX_RESERVED1_START + 1] = 0x00;

                // 3. PLC 정보 (2바이트): 클라이언트/서버 정보에 따라 설정
                byte[] plcInfoBytes = BitConverter.GetBytes(plcInfo);
                header[IDX_PLC_INFO_START] = plcInfoBytes[0];      // 상위 바이트
                header[IDX_PLC_INFO_START + 1] = plcInfoBytes[1];  // 하위 바이트

                // 4. CPU 정보 (1바이트): CPU 타입에 따라 설정
                header[IDX_CPU_INFO] = (byte)cpuType;

                // 5. 프레임 소스 (1바이트): 클라이언트/서버 모드에 따라 설정
                header[IDX_SOURCE_OF_FRAME] = (byte)mode;

                // 6. Invoke ID (2바이트): 프레임 순서 구분용
                byte[] invokeidBytes = BitConverter.GetBytes(invokeId);
                header[IDX_INVOKE_ID_START] = invokeidBytes[0];     // 상위 바이트
                header[IDX_INVOKE_ID_START + 1] = invokeidBytes[1]; // 하위 바이트

                // 7. 데이터 길이 (2바이트)
                byte[] lengthBytes = BitConverter.GetBytes((ushort)length);
                header[IDX_LENGTH_START] = lengthBytes[0];      // 상위 바이트
                header[IDX_LENGTH_START + 1] = lengthBytes[1];  // 하위 바이트

                // 8. FEnet 위치 (1바이트): 모듈 슬롯 번호
                header[IDX_FENET_POSITION] = fenetPosition;

                // 9. BCC 체크썸 계산 (1바이트)
                byte[] tempForBcc = new byte[HEADER_SIZE - 1]; // 마지막 BCC 필드 제외
                Array.Copy(header, 0, tempForBcc, 0, HEADER_SIZE - 1);
                byte[] bcc = CalculateBCC(tempForBcc);
                header[IDX_BCC] = bcc[0];

                return header;
            }
            catch (Exception ex)
            {
                throw new Exception("헤더 데이터 생성 중 오류가 발생했습니다.", ex);
            }
        }

     
        public static byte[] CreateClientHeader(PLCCpuType cpuType, int length, byte fenetPosition, ushort invokeId = 1, ushort plcInfo = 0)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "데이터 길이는 0 이상이어야 합니다.");
            }

            return MakeHeader(length, invokeId, plcInfo, fenetPosition, cpuType, CommunicationMode.Client);
        }

        public static byte[] CreateClientPacket(PLCCpuType cpuType, List<byte> data, byte fenetPosition, ushort invokeId = 1, ushort plcInfo = 0)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "바이트 리스트가 null일 수 없습니다.");
            }

            // 헤더 생성
            byte[] header = MakeHeader(data.Count, invokeId, plcInfo, fenetPosition, cpuType, CommunicationMode.Client);

            // 완성된 패킷을 담을 배열 생성 (헤더 + 데이터)
            byte[] packet = new byte[HEADER_SIZE + data.Count];

            // 헤더 복사
            Array.Copy(header, 0, packet, 0, HEADER_SIZE);

            // 데이터 복사
            byte[] dataArray = data.ToArray();
            Array.Copy(dataArray, 0, packet, HEADER_SIZE, data.Count);


            return packet;
        }


        public static byte[] CreateClientPacket(PLCCpuType cpuType, byte[] data, byte fenetPosition, ushort invokeId = 1, ushort plcInfo = 0)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "바이트 배열이 null일 수 없습니다.");
            }

            // 헤더 생성
            byte[] header = MakeHeader(data.Length, invokeId, plcInfo, fenetPosition, cpuType, CommunicationMode.Client);

            // 완성된 패킷을 담을 배열 생성 (헤더 + 데이터)
            byte[] packet = new byte[HEADER_SIZE + data.Length];

            // 헤더 복사
            Array.Copy(header, 0, packet, 0, HEADER_SIZE);

            // 데이터 복사
            Array.Copy(data, 0, packet, HEADER_SIZE, data.Length);

            return packet;
        }

        private static byte[] CalculateBCC(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.Length == 0)
            {
                return null;
            }

            byte[] bcc = new byte[1];
            bcc[0] = 0;

            try
            {
                for (int i = 0; i < data.Length; i++)
                {
                    bcc[0] ^= data[i];
                }
                return bcc;
            }
            catch (Exception ex)
            {
                throw new Exception("체크썸 계산 중 오류가 발생했습니다.", ex);
            }
        }
    }
}
