using System;
using LSELEC.Data;

namespace LSELEC
{
    /// <summary>
    /// PLC 생산 데이터 - 생산 관련 데이터 래퍼 클래스
    /// </summary>
    public class PLCProductionData
    {
        /// <summary>
        /// 생산 데이터
        /// </summary>
        public ProductionData? ProductionData { get; set; }

        /// <summary>
        /// 품질 데이터
        /// </summary>
        public QualityData? QualityData { get; set; }

        /// <summary>
        /// 장비 데이터
        /// </summary>
        public EquipmentData? EquipmentData { get; set; }

        /// <summary>
        /// 타임스탬프
        /// </summary>
        public DateTime Timestamp { get; set; }

        public PLCProductionData()
        {
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// 생산 효율 계산
        /// </summary>
        public double CalculateEfficiency()
        {
            if (ProductionData == null)
                return 0;

            var totalCount = ProductionData.ProductCount + ProductionData.DefectCount;
            if (totalCount == 0)
                return 0;

            return (double)ProductionData.ProductCount / totalCount * 100;
        }

        /// <summary>
        /// 불량률 계산
        /// </summary>
        public double CalculateDefectRate()
        {
            if (ProductionData == null)
                return 0;

            var totalCount = ProductionData.ProductCount + ProductionData.DefectCount;
            if (totalCount == 0)
                return 0;

            return (double)ProductionData.DefectCount / totalCount * 100;
        }

        /// <summary>
        /// 데이터 유효성 검증
        /// </summary>
        public bool IsValid()
        {
            return ProductionData != null || QualityData != null || EquipmentData != null;
        }

        /// <summary>
        /// 데이터를 문자열로 변환
        /// </summary>
        public override string ToString()
        {
            return $"PLCProductionData [Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}, " +
                   $"Efficiency: {CalculateEfficiency():F2}%, " +
                   $"DefectRate: {CalculateDefectRate():F2}%]";
        }
    }
}
