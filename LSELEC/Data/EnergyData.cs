using System;
using System.Collections.Generic;

namespace LSELEC.Data
{
    /// <summary>
    /// 에너지 데이터 클래스
    /// </summary>
    public class EnergyData
    {
        /// <summary>
        /// 타임스탬프
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 전력 소비량 (kWh)
        /// </summary>
        public double PowerConsumption { get; set; }

        /// <summary>
        /// 전압 (V)
        /// </summary>
        public double Voltage { get; set; }

        /// <summary>
        /// 전류 (A)
        /// </summary>
        public double Current { get; set; }

        /// <summary>
        /// 역률
        /// </summary>
        public double PowerFactor { get; set; }

        /// <summary>
        /// 주파수 (Hz)
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// 누적 전력 소비량 (kWh)
        /// </summary>
        public double TotalEnergyConsumption { get; set; }

        /// <summary>
        /// 피크 전력 (kW)
        /// </summary>
        public double PeakPower { get; set; }

        /// <summary>
        /// 측정 위치/장비 ID
        /// </summary>
        public string? MeasurementPoint { get; set; }

        /// <summary>
        /// 추가 정보
        /// </summary>
        public Dictionary<string, object>? AdditionalInfo { get; set; }

        public EnergyData()
        {
            Timestamp = DateTime.Now;
            PowerFactor = 1.0;
            Frequency = 60.0;
        }

        /// <summary>
        /// 전력(W) 계산
        /// </summary>
        public double CalculatePower()
        {
            return Voltage * Current * PowerFactor;
        }

        /// <summary>
        /// 데이터를 문자열로 변환
        /// </summary>
        public override string ToString()
        {
            return $"EnergyData [Power: {PowerConsumption:F2} kWh, Voltage: {Voltage:F2} V, Current: {Current:F2} A, Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}]";
        }
    }
}
