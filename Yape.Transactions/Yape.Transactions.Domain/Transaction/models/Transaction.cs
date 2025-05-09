﻿namespace Yape.Transactions.Domain.Transaction.models
{
    public enum TransactionStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid SourceAccountId { get; set; }
        public Guid TargetAccountId { get; set; }
        public int TransferTypeId { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public TransactionStatus Status { get; set; }
    }
}
