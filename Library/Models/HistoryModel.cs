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
            return historyModel;
        }
    }
}
