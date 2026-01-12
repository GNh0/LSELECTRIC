using System;
using System.Collections.Generic;

namespace LSELEC.Data
{
    /// <summary>
    /// 장비 데이터 클래스
    /// </summary>
    public class EquipmentData
    {
        /// <summary>
        /// 장비 상태
        /// </summary>
        public enum EquipmentStatus
        {
            Stopped,
            Running,
            Idle,
            Maintenance,
            Error,
            Emergency
        }

        /// <summary>
        /// 타임스탬프
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 장비 ID
        /// </summary>
        public string EquipmentId { get; set; } = string.Empty;

        /// <summary>
        /// 장비 이름
        /// </summary>
        public string? EquipmentName { get; set; }

        /// <summary>
        /// 장비 상태
        /// </summary>
        public EquipmentStatus Status { get; set; }

        /// <summary>
        /// 온도 (°C)
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// 압력 (bar)
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// 진동 (mm/s)
        /// </summary>
        public double Vibration { get; set; }

        /// <summary>
        /// 속도 (RPM)
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// 가동 시간 (시간)
        /// </summary>
        public double OperatingHours { get; set; }

        /// <summary>
        /// 가동률 (%)
        /// </summary>
        public double Utilization { get; set; }

        /// <summary>
        /// 마지막 유지보수 일자
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// 추가 정보
        /// </summary>
        public Dictionary<string, object>? AdditionalInfo { get; set; }

        public EquipmentData()
        {
            Timestamp = DateTime.Now;
            Status = EquipmentStatus.Stopped;
        }

        /// <summary>
        /// 장비 이상 여부 확인
        /// </summary>
        public bool IsAbnormal()
        {
            if (Status == EquipmentStatus.Error || Status == EquipmentStatus.Emergency)
                return true;

            if (Temperature > 80) // 온도 임계값
                return true;

            if (Vibration > 10) // 진동 임계값
                return true;

            return false;
        }

        /// <summary>
        /// 유지보수 필요 여부 확인
        /// </summary>
        public bool NeedsMaintenance(int maintenanceIntervalDays = 90)
        {
            if (LastMaintenanceDate == null)
                return true;

            return (DateTime.Now - LastMaintenanceDate.Value).TotalDays > maintenanceIntervalDays;
        }

        /// <summary>
        /// 데이터를 문자열로 변환
        /// </summary>
        public override string ToString()
        {
            return $"EquipmentData [ID: {EquipmentId}, Status: {Status}, Temp: {Temperature:F1}°C, Utilization: {Utilization:F1}%, Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}]";
        }
    }
}
