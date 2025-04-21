namespace SharePay.Models
{
    public class Expenses
    {
        public int? id { get; set; }
        public string? Note { get; set; }
        public string? name { get; set; }
        public bool? isSettled { get; set; }
        public int? amount { get; set; }
        public int? paidBy { get; set; }
        public string? paidByName { get; set; }
    }
}
