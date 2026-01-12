using System;
using System.Collections.Generic;

namespace LSELEC.Data
{
    /// <summary>
    /// 알람 데이터 클래스
    /// </summary>
    public class AlarmData
    {
        /// <summary>
        /// 알람 심각도
        /// </summary>
        public enum AlarmSeverity
        {
            Info,
            Warning,
            Error,
            Critical
        }

        /// <summary>
        /// 타임스탬프
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 알람 코드
        /// </summary>
        public int AlarmCode { get; set; }

        /// <summary>
        /// 알람 메시지
        /// </summary>
        public string AlarmMessage { get; set; } = string.Empty;

        /// <summary>
        /// 알람 심각도
        /// </summary>
        public AlarmSeverity Severity { get; set; }

        /// <summary>
        /// 알람 발생 위치
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// 알람 해제 여부
        /// </summary>
        public bool IsAcknowledged { get; set; }

        /// <summary>
        /// 알람 해제 시간
        /// </summary>
        public DateTime? AcknowledgedTime { get; set; }

        /// <summary>
        /// 추가 정보
        /// </summary>
        public Dictionary<string, object>? AdditionalInfo { get; set; }

        public AlarmData()
        {
            Timestamp = DateTime.Now;
            IsAcknowledged = false;
        }

        /// <summary>
        /// 알람 해제
        /// </summary>
        public void Acknowledge()
        {
            IsAcknowledged = true;
            AcknowledgedTime = DateTime.Now;
        }

        /// <summary>
        /// 알람을 문자열로 변환
        /// </summary>
        public override string ToString()
        {
            return $"AlarmData [Code: {AlarmCode}, Severity: {Severity}, Message: {AlarmMessage}, Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}]";
        }
    }
}
