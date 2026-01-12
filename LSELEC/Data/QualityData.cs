using System;
using System.Collections.Generic;

namespace LSELEC.Data
{
    /// <summary>
    /// 품질 데이터 클래스
    /// </summary>
    public class QualityData
    {
        /// <summary>
        /// 검사 결과
        /// </summary>
        public enum InspectionStatus
        {
            Pass,
            Fail,
            Pending,
            Rework
        }

        /// <summary>
        /// 타임스탬프
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 불량률 (%)
        /// </summary>
        public double DefectRate { get; set; }

        /// <summary>
        /// 품질 점수 (0-100)
        /// </summary>
        public double QualityScore { get; set; }

        /// <summary>
        /// 검사 결과
        /// </summary>
        public InspectionStatus InspectionResult { get; set; }

        /// <summary>
        /// 검사 항목 수
        /// </summary>
        public int InspectionItemCount { get; set; }

        /// <summary>
        /// 합격 항목 수
        /// </summary>
        public int PassedItemCount { get; set; }

        /// <summary>
        /// 불합격 항목 수
        /// </summary>
        public int FailedItemCount { get; set; }

        /// <summary>
        /// 검사자 ID
        /// </summary>
        public string? InspectorId { get; set; }

        /// <summary>
        /// 제품 ID
        /// </summary>
        public string? ProductId { get; set; }

        /// <summary>
        /// 불량 유형
        /// </summary>
        public List<string>? DefectTypes { get; set; }

        /// <summary>
        /// 추가 정보
        /// </summary>
        public Dictionary<string, object>? AdditionalInfo { get; set; }

        public QualityData()
        {
            Timestamp = DateTime.Now;
            InspectionResult = InspectionStatus.Pending;
            DefectTypes = new List<string>();
        }

        /// <summary>
        /// 검사 합격률 계산 (%)
        /// </summary>
        public double CalculatePassRate()
        {
            if (InspectionItemCount == 0)
                return 0;

            return (double)PassedItemCount / InspectionItemCount * 100;
        }

        /// <summary>
        /// 검사 불합격률 계산 (%)
        /// </summary>
        public double CalculateFailRate()
        {
            if (InspectionItemCount == 0)
                return 0;

            return (double)FailedItemCount / InspectionItemCount * 100;
        }

        /// <summary>
        /// 품질 등급 평가
        /// </summary>
        public string EvaluateQualityGrade()
        {
            if (QualityScore >= 95)
                return "A+";
            else if (QualityScore >= 90)
                return "A";
            else if (QualityScore >= 80)
                return "B";
            else if (QualityScore >= 70)
                return "C";
            else if (QualityScore >= 60)
                return "D";
            else
                return "F";
        }

        /// <summary>
        /// 데이터를 문자열로 변환
        /// </summary>
        public override string ToString()
        {
            return $"QualityData [Score: {QualityScore:F1}, DefectRate: {DefectRate:F2}%, Grade: {EvaluateQualityGrade()}, Result: {InspectionResult}, Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}]";
        }
    }
}
