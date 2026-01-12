using System;
using System.Collections.Generic;

namespace LSELEC.Data
{
    /// <summary>
    /// 환경 데이터 클래스
    /// </summary>
    public class EnvironmentData
    {
        /// <summary>
        /// 타임스탬프
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 온도 (°C)
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// 습도 (%)
        /// </summary>
        public double Humidity { get; set; }

        /// <summary>
        /// 공기 질 지수
        /// </summary>
        public double AirQuality { get; set; }

        /// <summary>
        /// 기압 (hPa)
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// 소음 레벨 (dB)
        /// </summary>
        public double NoiseLevel { get; set; }

        /// <summary>
        /// 조도 (lux)
        /// </summary>
        public double Illuminance { get; set; }

        /// <summary>
        /// CO2 농도 (ppm)
        /// </summary>
        public double CO2Level { get; set; }

        /// <summary>
        /// 측정 위치
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// 추가 정보
        /// </summary>
        public Dictionary<string, object>? AdditionalInfo { get; set; }

        public EnvironmentData()
        {
            Timestamp = DateTime.Now;
            Pressure = 1013.25; // 표준 대기압
        }

        /// <summary>
        /// 환경 상태 평가
        /// </summary>
        public string EvaluateCondition()
        {
            if (Temperature < 15 || Temperature > 30)
                return "온도 비정상";
            if (Humidity < 30 || Humidity > 70)
                return "습도 비정상";
            if (AirQuality > 150)
                return "공기질 나쁨";

            return "정상";
        }

        /// <summary>
        /// 데이터를 문자열로 변환
        /// </summary>
        public override string ToString()
        {
            return $"EnvironmentData [Temp: {Temperature:F1}°C, Humidity: {Humidity:F1}%, AirQuality: {AirQuality:F0}, Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}]";
        }
    }
}
