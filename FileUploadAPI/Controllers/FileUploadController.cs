using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace FileUploadAPI.Controllers
{
    public class FileUploadController : ApiController
    {
        [HttpPost]
        public bool UploadFile()
        {
            //ResponseModel res = new ResponseModel();
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                var httpRequest = HttpContext.Current.Request;
                string imgPath = "";
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        imgPath = "/Content/CsvFiles/" + postedFile.FileName;
                        string filePath = HttpContext.Current.Server.MapPath("~/Content/CsvFiles/" + postedFile.FileName);
                        //postedFile.SaveAs(filePath);
                    }
                }
                //res.data = imgPath;
                //res.message = "";
                //res.status = true;
                return true;
            }
            catch (Exception ex)
            {
                //res.data = ex.InnerException;
                //res.message = ex.Message;
                //res.status = false;
                return false;
            }

        }

        [HttpGet]
        public string GetTest()
        {
            return "I am Get API";
        }
    }
}