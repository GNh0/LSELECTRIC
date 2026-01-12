using System;
using System.Collections.Generic;
using System.Text;

namespace LSELEC.Communication
{
    /// <summary>
    /// FEnet 명령어 클래스
    /// </summary>
    public class FEnetInstruction
    {
        /// <summary>
        /// 명령어 타입
        /// </summary>
        public enum InstructionType : byte
        {
            Read = 0x54,           // 읽기
            Write = 0x58,          // 쓰기
            ReadMultiple = 0x14,   // 다중 읽기
            WriteMultiple = 0x18   // 다중 쓰기
        }

        /// <summary>
        /// 데이터 타입
        /// </summary>
        public enum DataType : byte
        {
            Bit = 0x00,
            Byte = 0x01,
            Word = 0x02,
            DWord = 0x03,
            LWord = 0x04
        }

        /// <summary>
        /// 명령어 타입
        /// </summary>
        public InstructionType Type { get; set; }

        /// <summary>
        /// 데이터 타입
        /// </summary>
        public DataType Data { get; set; }

        /// <summary>
        /// 디바이스 주소
        /// </summary>
        public string DeviceAddress { get; set; } = string.Empty;

        /// <summary>
        /// 읽기/쓰기 개수
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 쓰기 데이터
        /// </summary>
        public byte[]? WriteData { get; set; }

        public FEnetInstruction()
        {
            Type = InstructionType.Read;
            Data = DataType.Word;
            Count = 1;
        }

        /// <summary>
        /// 명령어 바이트 배열 생성
        /// </summary>
        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            // 명령어 타입
            bytes.Add((byte)Type);

            // 데이터 타입
            bytes.Add((byte)Data);

            // 디바이스 주소 (ASCII 인코딩)
            var addressBytes = Encoding.ASCII.GetBytes(DeviceAddress);
            bytes.AddRange(addressBytes);

            // 개수 (2바이트, 리틀 엔디안)
            bytes.Add((byte)(Count & 0xFF));
            bytes.Add((byte)((Count >> 8) & 0xFF));

            // 쓰기 명령인 경우 데이터 추가
            if ((Type == InstructionType.Write || Type == InstructionType.WriteMultiple) && WriteData != null)
            {
                bytes.AddRange(WriteData);
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// 읽기 명령어 생성
        /// </summary>
        public static FEnetInstruction CreateReadInstruction(string deviceAddress, int count = 1, DataType dataType = DataType.Word)
        {
            return new FEnetInstruction
            {
                Type = count == 1 ? InstructionType.Read : InstructionType.ReadMultiple,
                Data = dataType,
                DeviceAddress = deviceAddress,
                Count = count
            };
        }

        /// <summary>
        /// 쓰기 명령어 생성
        /// </summary>
        public static FEnetInstruction CreateWriteInstruction(string deviceAddress, byte[] data, DataType dataType = DataType.Word)
        {
            return new FEnetInstruction
            {
                Type = InstructionType.Write,
                Data = dataType,
                DeviceAddress = deviceAddress,
                Count = data.Length / GetDataTypeSize(dataType),
                WriteData = data
            };
        }

        /// <summary>
        /// 데이터 타입 크기 반환 (바이트)
        /// </summary>
        private static int GetDataTypeSize(DataType dataType)
        {
            return dataType switch
            {
                DataType.Bit => 1,
                DataType.Byte => 1,
                DataType.Word => 2,
                DataType.DWord => 4,
                DataType.LWord => 8,
                _ => 2
            };
        }

        /// <summary>
        /// 명령어를 문자열로 변환
        /// </summary>
        public override string ToString()
        {
            return $"FEnetInstruction [Type: {Type}, DataType: {Data}, Address: {DeviceAddress}, Count: {Count}]";
        }
    }
}
