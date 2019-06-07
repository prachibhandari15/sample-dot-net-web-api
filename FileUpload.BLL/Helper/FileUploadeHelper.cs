using CsvHelper;
using FileUpload.BLL.Model;
using FileUpload.Entity.EntityModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FileUpload.BLL.Helper
{
    public class FileUploadeHelper
    {
        public ResponseModel UploadCustomerOrder(System.Web.HttpRequest httpRequest, string rootPath)
        {
            ResponseModel res = new ResponseModel();
            try
            {
                if (httpRequest.Files.Count > 0)
                {
                    foreach (string file in httpRequest.Files)
                    {
                        HttpPostedFile fileUploaded = httpRequest.Files[file];
                        string fileName = UtilityHelper.SaveUploadedFile(fileUploaded, rootPath);
                        string filePath = Path.Combine(rootPath, fileName);
                        if (fileName != "")
                        {
                            using (var reader = new StreamReader(filePath))
                            using (var csv = new CsvReader(reader))
                            {
                                int rowCnt = 0;
                                var record = new CustomerOrderModel();

                                var records = csv.EnumerateRecords(record);
                                
                                using (CustomerDBEntities db = new CustomerDBEntities())
                                {
                                    FileStorageInfo fileInfo = new FileStorageInfo();
                                    fileInfo.FileUploadId = Guid.NewGuid();
                                    fileInfo.OriginalFileName = fileUploaded.FileName;
                                    fileInfo.StoredFileName = fileName;
                                    fileInfo.CreatedDate = DateTime.Now;
                                    db.FileStorageInfoes.Add(fileInfo);
                                    db.SaveChanges();

                                    foreach (var r in records)
                                    {
                                        TempCustomerOrder tmpOrder = new TempCustomerOrder();
                                        tmpOrder.ID = Guid.NewGuid();
                                        tmpOrder.order_number = r.order_number;
                                        tmpOrder.qty = r.qty;
                                        tmpOrder.user_id = r.user_id;
                                        tmpOrder.FileUploadedId = fileInfo.FileUploadId;
                                        tmpOrder.CreatedDate = DateTime.Now;
                                        tmpOrder.IsAddedToMainTbl = false;
                                        db.TempCustomerOrders.Add(tmpOrder);
                                        db.SaveChanges();

                                        rowCnt++;
                                    }
                                    
                                }
                                if (rowCnt > 0)
                                {
                                    //Upsert Customer Order Data from Staginig table to Main Table
                                    res = UpsertOrderData();
                                    if (res.status)
                                    {
                                        res.status = true;
                                        res.message = "File Upload Completed Successfully...!!!";
                                        res.data = GetAllCustomerOrders();
                                    }
                                    return res;
                                }
                                else
                                {
                                    res.message = "No/Invalid Data in File...!!!";
                                    return res;
                                }
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                if (ex.Source == "CsvHelper")
                {
                    res.message = "Invalid File Data...!!!";
                }
                else
                {
                    res.message = ex.Message;
                }
                res.data = ex.InnerException;
                return res;
            }
        }

        public ResponseModel UpsertOrderData()
        {
            ResponseModel res = new ResponseModel();
            try
            {
                //BackUp Customer Order Data to Backup Table in Database
                res = BackupCustomerOrderData();
                if (res.status)
                {
                    //Upsert Customer Order Data
                    using (CustomerDBEntities db = new CustomerDBEntities())
                    {

                        List<TempCustomerOrder> lstTmpCustomerOrders = db.TempCustomerOrders.Where(x => x.IsAddedToMainTbl != true).ToList();

                        foreach (var tmpOrder in lstTmpCustomerOrders)
                        {
                            CustomerOrder customerOrder = db.CustomerOrders.Where(x => x.user_id == tmpOrder.user_id && x.order_number == tmpOrder.order_number).FirstOrDefault();

                            if (customerOrder != null)
                            {
                                customerOrder.qty = tmpOrder.qty;
                                customerOrder.FileUploadedId = tmpOrder.FileUploadedId;
                                customerOrder.UpdatedDate = DateTime.Now;
                                customerOrder.IsAddedToBkpTbl = false;
                                db.SaveChanges();
                            }
                            else
                            {
                                customerOrder = new CustomerOrder();
                                customerOrder.ID = Guid.NewGuid();
                                customerOrder.order_number = tmpOrder.order_number;
                                customerOrder.qty = tmpOrder.qty;
                                customerOrder.user_id = tmpOrder.user_id;
                                customerOrder.FileUploadedId = tmpOrder.FileUploadedId;
                                customerOrder.CreatedDate = DateTime.Now;
                                customerOrder.IsAddedToBkpTbl = false;
                                db.CustomerOrders.Add(customerOrder);
                                db.SaveChanges();
                            }
                            tmpOrder.IsAddedToMainTbl = true;
                            db.SaveChanges();
                        }
                    }
                    res.status = true;
                }
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.data = ex.InnerException;
            }
            return res;
        }

        public ResponseModel BackupCustomerOrderData()
        {
            ResponseModel res = new ResponseModel();
            try
            {
                using (CustomerDBEntities db = new CustomerDBEntities())
                {
                    List<CustomerOrder> lstCustomerOrders = db.CustomerOrders.Where(x => x.IsAddedToBkpTbl != true).ToList();

                    foreach (var order in lstCustomerOrders)
                    {
                        BkpCustomerOrder customerOrder = new BkpCustomerOrder();
                        customerOrder.BkpID = Guid.NewGuid();
                        customerOrder.order_number = order.order_number;
                        customerOrder.qty = order.qty;
                        customerOrder.user_id = order.user_id;
                        customerOrder.FileUploadedId = order.FileUploadedId;
                        customerOrder.BkpDate = DateTime.Now;
                        db.BkpCustomerOrders.Add(customerOrder);
                        db.SaveChanges();

                        order.IsAddedToBkpTbl = true;
                        db.SaveChanges();
                    }
                }
                res.status = true;
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.data = ex.InnerException;
            }
            return res;
        }

        public List<CustomerOrder> GetAllCustomerOrders()
        {
            CustomerDBEntities db = new CustomerDBEntities();
            return db.CustomerOrders.ToList();
        }
    }
}
