using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Models
{
    public class HistoryModel
    {
        private const double _second60 = 0.0006944444394321181;
        public List<(double x, double y)> PointsIsBuyer { get; set; } = new();
        public List<(double x, double y)> PointsIsMaker { get; set; } = new();
        public List<(double x, double y)> PointsIsOpenLong { get; set; } = new();
        public List<(double x, double y)> PointsIsOpenShort { get; set; } = new();
        public List<(double x, double y)> PointsIsClosePositive { get; set; } = new();
        public List<(double x, double y)> PointsIsCloseNegative { get; set; } = new();
        public List<(double[] x, double[] y)> LineIsUpperBuffer { get; set; } = new();
        public List<(double[] x, double[] y)> LineIsLowerBuffer { get; set; } = new();
        public List<(double[] x, double[] y)> LineIsUpperDistance { get; set; } = new();
        public List<(double[] x, double[] y)> LineIsLowerDistance { get; set; } = new();
        public HistoryModel Get(DateTime open, DateTime close)
        {
            double openTime = open.ToOADate();
            double closeTime = close.ToOADate();
            double start = openTime - _second60;
            double end = closeTime + _second60;
            HistoryModel historyModel = new();
            historyModel.PointsIsMaker = PointsIsMaker.Where(item => item.x > start && item.x < end).ToList();
            historyModel.PointsIsBuyer = PointsIsBuyer.Where(item => item.x > start && item.x < end).ToList();
            historyModel.PointsIsOpenLong = PointsIsOpenLong.Where(item => item.x == openTime).ToList();
            historyModel.PointsIsOpenShort = PointsIsOpenShort.Where(item => item.x == openTime).ToList();
            historyModel.PointsIsClosePositive = PointsIsClosePositive.Where(item => item.x == closeTime).ToList();
            historyModel.PointsIsCloseNegative = PointsIsCloseNegative.Where(item => item.x == closeTime).ToList();
            historyModel.LineIsLowerBuffer = LineIsLowerBuffer.Where(item => item.x[1] > start && item.x[0] < end).ToList();
            historyModel.LineIsUpperBuffer = LineIsUpperBuffer.Where(item => item.x[1] > start && item.x[0] < end).ToList();
            historyModel.LineIsLowerDistance = LineIsLowerDistance.Where(item => item.x[1] > start && item.x[0] < end).ToList();
            historyModel.LineIsUpperDistance = LineIsUpperDistance.Where(item => item.x[1] > start && item.x[0] < end).ToList();
            return historyModel;
        }
        public void AddLines(double UpperBuffer, double LowerBuffer, double UpperDistance, double LowerDistance)
        {
            double[] x = new double[2];
            x[0] = x[1] = DateTime.UtcNow.ToOADate();

            double[] upperBuffers = new double[2];
            upperBuffers[0] = upperBuffers[1] = UpperBuffer;
            LineIsUpperBuffer.Add((x, upperBuffers));

            double[] lowerBuffers = new double[2];
            lowerBuffers[0] = lowerBuffers[1] = LowerBuffer;
            LineIsLowerBuffer.Add((x, lowerBuffers));

            double[] upperDistances = new double[2];
            upperDistances[0] = upperDistances[1] = UpperDistance;
            LineIsUpperDistance.Add((x, upperDistances));

            double[] lowerDistances = new double[2];
            lowerDistances[0] = lowerDistances[1] = LowerDistance;
            LineIsLowerDistance.Add((x, lowerDistances));
        }
        public void UpdateTimeLine()
        {
            double dateTime = DateTime.UtcNow.ToOADate();
            if(LineIsLowerDistance.Count > 0)
            {
                LineIsUpperBuffer[LineIsUpperBuffer.Count - 1].x[1] = dateTime;
                LineIsLowerBuffer[LineIsLowerBuffer.Count - 1].x[1] = dateTime;
                LineIsUpperDistance[LineIsUpperDistance.Count - 1].x[1] = dateTime;
                LineIsLowerDistance[LineIsLowerDistance.Count - 1].x[1] = dateTime;
            }
        }
    }
}
