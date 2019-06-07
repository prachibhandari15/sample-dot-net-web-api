using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FileUpload.BLL.Helper
{
    public class UtilityHelper
    {
        public static double ConvertToDoubleVal(string value)
        {
            if (value != "")
            {
                return Convert.ToDouble(value);
            }
            return 0;
        }

        public static string SaveUploadedFile(HttpPostedFile file, string rootPath)
        {

            try
            {
                if (!string.IsNullOrEmpty(file.FileName))
                {
                    string fileExt = System.IO.Path.GetExtension(file.FileName);

                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);
                    }
                    string fileName = Guid.NewGuid() + fileExt.ToLower();
                    if (file.ContentLength > 0)
                    {
                        var filePath = Path.Combine(rootPath, fileName);
                        if (file.FileName.Contains("csv"))
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                file.InputStream.CopyTo(fileStream);
                                fileStream.Dispose();
                            }
                        }
                        else
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                file.InputStream.CopyTo(fileStream);
                                fileStream.Dispose();
                            }
                        }
                    }
                    return fileName;
                }
            }
            catch (Exception ex)
            {
                //LogWriter.writeLog(ex.Message);
                return "";
            }
            return "";
        }
    }
}
