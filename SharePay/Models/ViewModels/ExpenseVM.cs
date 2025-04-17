namespace SharePay.Models.ViewModels
{
    public class ExpenseVM
    {
        public int? id { get; set; }
        public string? Note { get; set; }
        public string? name { get; set; }
        public bool? isSettled { get; set; }
        public int? amount { get; set; }
        public List<int>? users { get; set; }
        public int? paidBy { get; set; }
    }
}
