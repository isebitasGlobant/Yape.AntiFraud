using System.ComponentModel.DataAnnotations;

namespace Yape.AntiFraud.AdapterInHttp.DTOs
{
    public class TransactionUpdateMessageRequest
    {
        public Guid Id { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [StringLength(10)]
        public string Status { get; set; }
    }
}