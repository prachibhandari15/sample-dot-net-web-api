using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BLL = FileUpload.BLL;

namespace FileUploadAPI.Controllers
{
    public class FileUploadController : ApiController
    {
        private readonly HttpContext _context;

        public FileUploadController()
        {
            _context = HttpContext.Current;
        }

        [HttpPost]
        public BLL.Model.ResponseModel UploadFile()
        {
            BLL.Model.ResponseModel res = new BLL.Model.ResponseModel();
            try
            {
                BLL.Helper.FileUploadeHelper _helper = new BLL.Helper.FileUploadeHelper();
                HttpResponseMessage response = new HttpResponseMessage();
                var httpRequest = _context.Request;
                string filePath = HttpContext.Current.Server.MapPath("~/Content/OrderCsvFiles");
                if (httpRequest.Files.Count > 0)
                {
                    res = _helper.UploadCustomerOrder(httpRequest, filePath);
                }
                return res;
            }
            catch (Exception ex)
            {
                res.data = ex.InnerException;
                res.message = ex.Message;
                res.status = false;
                return res;
            }
        }

        [HttpGet]
        public string GetTest()
        {
            return "I am Get API";
        }
    }
}