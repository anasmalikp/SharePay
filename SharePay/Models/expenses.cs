namespace SharePay.Models
{
    public class expenses
    {
        public int? id { get; set; }
        public string? Note { get; set; }
        public string? name { get; set; }
        public bool? isSettled { get; set; }
        public int? amount { get; set; }
    }
}
