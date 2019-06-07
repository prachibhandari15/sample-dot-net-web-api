using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.BLL.Model
{
    public class CustomerOrderModel
    {
        public string order_number { get; set; }
        public int? qty { get; set; }
        public string user_id { get; set; }
    }
}
