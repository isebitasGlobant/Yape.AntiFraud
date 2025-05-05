namespace Yape.Transactions.AdapterInHttp.DTOs
{
    public class TransactionRequest
    {
        public Guid SourceAccountId { get; set; }
        public Guid TargetAccountId { get; set; }
        public int TransferTypeId { get; set; }
        public decimal Value { get; set; }
    }

    public class TransactionUpdateMessageRequest
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }
}