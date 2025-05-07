namespace Yape.Transactions.AdapterOutRepository.postgreSql.entities
{
    public class TransactionEntity
    {
        public Guid Id { get; set; }
        public Guid SourceAccountId { get; set; }
        public Guid TargetAccountId { get; set; }
        public int TransferTypeId { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } // "pending", "approved", "rejected"
    }
}
