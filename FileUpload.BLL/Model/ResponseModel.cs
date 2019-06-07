using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.BLL.Model
{
    public class ResponseModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public object data { get; set; }

        public ResponseModel()
        {
            status = false;
            message = "";
            data = null;
        }
    }
}
