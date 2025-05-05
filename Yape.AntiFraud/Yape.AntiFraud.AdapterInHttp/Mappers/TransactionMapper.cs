using Yape.AntiFraud.AdapterInHttp.DTOs;
using Yape.AntiFraud.Domain.Transaction.models;

namespace Yape.AntiFraud.AdapterInHttp.Mappers
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
