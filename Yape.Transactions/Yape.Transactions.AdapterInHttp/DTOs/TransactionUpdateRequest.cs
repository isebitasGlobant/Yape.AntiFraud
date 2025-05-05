using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yape.AntiFraud.AdapterInHttp.DTOs
{
    namespace Yape.AntiFraud.AdapterInHttp.DTOs
    {
        public class TransactionUpdateRequest
        {
            public Guid TransactionId { get; set; }
            public string Status { get; set; }
        }
    }

}
