namespace OrderShotLibrary.Models
{
    public class ClientData
    {
        public string Symbol { get; set; }
        public double Distance { get; set; }
        public double Buffer { get; set; }
        public double TakeProfit { get; set; }
        public double StopLoss { get; set; }
        public int FollowPriceDelay { get; set; }
        public bool IsShort { get; set; }
        public bool IsLong { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is ClientData))
            {
                return false;
            }
            return (this.Symbol == ((ClientData)obj).Symbol)
                && (this.Distance == ((ClientData)obj).Distance)
                && (this.Buffer == ((ClientData)obj).Buffer)
                && (this.TakeProfit == ((ClientData)obj).TakeProfit)
                && (this.StopLoss == ((ClientData)obj).StopLoss)
                && (this.FollowPriceDelay == ((ClientData)obj).FollowPriceDelay)
                && (this.IsShort == ((ClientData)obj).IsShort)
                && (this.IsLong == ((ClientData)obj).IsLong);
        }
    }
}
