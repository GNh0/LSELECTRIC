using System;
using System.Collections.Generic;

namespace LSELEC.Data
{
    /// <summary>
    /// 생산 데이터 클래스
    /// </summary>
    public class ProductionData
    {
        /// <summary>
        /// 타임스탬프
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 생산량
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// 불량 수량
        /// </summary>
        public int DefectCount { get; set; }

        /// <summary>
        /// 사이클 타임 (초)
        /// </summary>
        public double CycleTime { get; set; }

        /// <summary>
        /// 목표 생산량
        /// </summary>
        public int TargetCount { get; set; }

        /// <summary>
        /// 작업 시작 시간
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 작업 종료 시간
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 작업자 ID
        /// </summary>
        public string? OperatorId { get; set; }

        /// <summary>
        /// 제품 코드
        /// </summary>
        public string? ProductCode { get; set; }

        /// <summary>
        /// 배치 번호
        /// </summary>
        public string? BatchNumber { get; set; }

        /// <summary>
        /// 추가 정보
        /// </summary>
        public Dictionary<string, object>? AdditionalInfo { get; set; }

        public ProductionData()
        {
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// 생산 달성률 계산 (%)
        /// </summary>
        public double CalculateAchievementRate()
        {
            if (TargetCount == 0)
                return 0;

            return (double)ProductCount / TargetCount * 100;
        }

        /// <summary>
        /// 불량률 계산 (%)
        /// </summary>
        public double CalculateDefectRate()
        {
            var totalCount = ProductCount + DefectCount;
            if (totalCount == 0)
                return 0;

            return (double)DefectCount / totalCount * 100;
        }

        /// <summary>
        /// 양품률 계산 (%)
        /// </summary>
        public double CalculateYieldRate()
        {
            var totalCount = ProductCount + DefectCount;
            if (totalCount == 0)
                return 0;

            return (double)ProductCount / totalCount * 100;
        }

        /// <summary>
        /// 작업 시간 계산 (분)
        /// </summary>
        public double CalculateWorkingTime()
        {
            if (StartTime == null || EndTime == null)
                return 0;

            return (EndTime.Value - StartTime.Value).TotalMinutes;
        }

        /// <summary>
        /// 시간당 생산량 계산
        /// </summary>
        public double CalculateProductionRate()
        {
            var workingTime = CalculateWorkingTime();
            if (workingTime == 0)
                return 0;

            return ProductCount / (workingTime / 60);
        }

        /// <summary>
        /// 데이터를 문자열로 변환
        /// </summary>
        public override string ToString()
        {
            return $"ProductionData [Count: {ProductCount}, Defects: {DefectCount}, DefectRate: {CalculateDefectRate():F2}%, Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}]";
        }
    }
}
