using Yape.AntiFraud.AdapterOutRepository.postgreSql.entities;
using Yape.AntiFraud.Domain.Transaction.models;

namespace Yape.AntiFraud.AdapterOutRepository.postgreSql.Mapper
{
    public static class TransactionMapper
    {
        public static Transaction? ToDomain(this TransactionEntity? entity)
        {
            if (entity is null)
                return null;

            return new Transaction
            {
                Id = entity.Id,
                SourceAccountId = entity.SourceAccountId,
                TargetAccountId = entity.TargetAccountId,
                TransferTypeId = entity.TransferTypeId,
                Value = entity.Value,
                Status = Enum.TryParse<TransactionStatus>(entity.Status, out var status) ? status : TransactionStatus.Pending,
                CreatedAt = entity.CreatedAt
            };
        }

        public static TransactionEntity ToEntity(this Transaction domain)
        {
            return new TransactionEntity
            {
                Id = domain.Id,
                SourceAccountId = domain.SourceAccountId,
                TargetAccountId = domain.TargetAccountId,
                TransferTypeId = domain.TransferTypeId,
                Value = domain.Value,
                Status = domain.Status.ToString(), // Convert enum to string
                CreatedAt = domain.CreatedAt
            };
        }
    }
}
