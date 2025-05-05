using Yape.Transactions.AdapterInHttp.DTOs;
using Yape.Transactions.Domain.Transaction.models;

namespace Yape.Transactions.AdapterInHttp.Mappers
{
    public static class TransactionMapper
    {
        public static Transaction ToDomain(this TransactionRequest request)
        {
            return new Transaction
            {
                SourceAccountId = request.SourceAccountId,
                TargetAccountId = request.TargetAccountId,
                Value = request.Value,
                TransferTypeId = request.TransferTypeId
            };
        }

        public static Transaction ToDomain(this TransactionUpdateMessageRequest request)
        {
            return new Transaction
            {
                Id = request.Id,
                Status = Enum.TryParse<TransactionStatus>(request.Status, out var status) ? status : TransactionStatus.Pending
            };
        }
    }
}
