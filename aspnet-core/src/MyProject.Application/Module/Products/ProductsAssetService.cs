namespace MyProject.Module.Products
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.IO.Packaging;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Text;
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using Abp.Auditing;
    using Abp.Authorization;
    using Abp.Domain.Repositories;
    using Abp.Linq.Extensions;
    using Abp.UI;
    using DbEntities;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using MyProject.Authorization;
    using MyProject.Authorization.Users;
    using MyProject.Data;
    using MyProject.Data.Excel.Dtos;
    using MyProject.Global;
    using MyProject.Global.Dtos;
    using MyProject.Net.MimeTypes;
    using OfficeOpenXml;
    using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
    using OfficeOpenXml.Style;
    using Microsoft.Extensions.Configuration;
    using MyProject.Module.Products.Dtos;
    using MyProject.DanhMuc.Staffs.Stos;
    using System.Data;

    [AbpAuthorize]
    public class ProductsAssetService : MyProjectAppServiceBase
    {
        private readonly IAppFolders appFolders;
        private readonly IWebHostEnvironment _env;
        private readonly IRepository<Products> productRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IConfiguration configurationApp;
        private readonly string connectionString;
        private readonly string database = string.Empty;
        private readonly string ProductDatabase;

        public ProductsAssetService(
            IConfiguration configurationApp,
            IAppFolders appFolders,
            IRepository<Products> productRepository,
            IRepository<User, long> userRepository,
            IWebHostEnvironment env
        )
        {
            this.connectionString = configurationApp["ConnectionStrings:Default"];
            this.database = this.GetDatabaseName(this.connectionString);
            this.ProductDatabase = $"[{this.database}].[dbo].[Products]";
            this.appFolders = appFolders;
            this.productRepository = productRepository;
            this._userRepository = userRepository;
            _env = env;
        }

        public async Task<PagedResultDto<GetAllOutputDto>> GetAllAsync(InputProductDto input)
        {
            try
            {
                if (input == null)
                {
                    throw new UserFriendlyException(StringResources.NullParameter);
                }

                input.Keyword = GlobalFunction.RegexFormat(input.Keyword);

                var query = from Pn in this.productRepository.GetAll()
                            .WhereIf(!string.IsNullOrEmpty(input.Keyword), e => e.ProductName.ToLower().Contains(input.Keyword.ToLower()) || e.Category.ToLower().Contains(input.Keyword.ToLower()))
                            select new GetAllOutputDto
                            {
                                Id = Pn.Id,
                                ProductName = Pn.ProductName,
                                Description = Pn.Description,
                                Price = Pn.Price,
                                Category = Pn.Category,
                                DateCreated = Pn.CreationTime,
                                UserName = Pn.LastModifierUserId == null ? this._userRepository.GetAll().FirstOrDefault(x => x.Id == Pn.CreatorUserId).UserName :
                                                this._userRepository.GetAll().FirstOrDefault(x => x.Id == Pn.LastModifierUserId).UserName,
                                LastDateModified = Pn.LastModificationTime,
                            };

                int totalCount = await query.CountAsync();
                var items = query.PageBy(input).ToList();
                return new PagedResultDto<GetAllOutputDto>
                {
                    TotalCount = totalCount,
                    Items = items,
                };
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }

        public async Task CreateProductAsync(CreateInputDto input)
        {
            var checkProduct = await this.productRepository.FirstOrDefaultAsync(p => p.ProductName.Equals(input.ProductName));

            if (checkProduct != null)
            {
                throw new UserFriendlyException("Product already exists!");
            }

            var CreateProduct = new Products
            {
                ProductName = input.ProductName.Trim(),
                Description = input.Description != null ? input.Description.Trim() : null,
                Price = input.Price,
                Category = input.Category.Trim(),
            };

            this.productRepository.Insert(CreateProduct);
            this.CurrentUnitOfWork.SaveChanges();
        }

        public async Task UpdateProductAsync(CreateInputDto input)
        {
            if (input == null)
            {
                throw new UserFriendlyException(StringResources.NullParameter);
            }

            var checkProduct = await this.productRepository.FirstOrDefaultAsync(p => p.ProductName.Equals(input.ProductName) && p.Id != input.Id);

            if (checkProduct != null)
            {
                throw new UserFriendlyException("Product already exists!");
            }
            else
            {
                var ProductAsset = await this.productRepository.FirstOrDefaultAsync(p => p.Id == input.Id);
                ProductAsset.ProductName = input.ProductName;
                ProductAsset.Description = input.Description;
                ProductAsset.Price = input.Price;
                ProductAsset.Category = input.Category;
                this.productRepository.Update(ProductAsset);
            }
        }

        public async Task<GetAllOutputDto> ViewProductAsync(int ProductId)
        {
            var checkProduct = this.productRepository.FirstOrDefault(w => w.Id == ProductId);

            if (checkProduct != null)
            {
                var ProductAsset = new GetAllOutputDto()
                {
                    ProductName = checkProduct.ProductName,
                    Description = checkProduct.Description,
                    Price = checkProduct.Price,
                    Category = checkProduct.Category,
                    Id = checkProduct.Id,
                };
                return ProductAsset;
            }
            else
            {
                throw new UserFriendlyException("There is already a Product with given Code");
            }
        }

        public async Task DeleteProductpAsync(int ProductId)
        {
            var newProduct = this.productRepository.FirstOrDefault(w => w.Id == ProductId);

            this.productRepository.Delete(newProduct);

        }

        public async Task<int> DeleteListProductpAsync(List<int> ProductId)
        {
            var newListProduct = await this.productRepository.GetAll().Where(w => ProductId.Contains(w.Id)).Select(e => e.Id).ToListAsync();
            int result = 0;

            var ProductToDelete = ProductId.Except(newListProduct);

            foreach (var item in ProductToDelete)
            {
                this.productRepository.Delete(item);
                result++;
            }

            return result;
        }

        public async Task<FileDto> DownloadFileMau()
        {
            string fileName = string.Format("Import_Product.xlsx");
            try
            {
                // _appFolders.DemoFileDownloadFolder : Thư mục chưa file mẫu cần tải
                // _appFolders.TempFileDownloadFolder : Không được sửa
                return await GlobalFunction.DownloadFileMau(fileName, this.appFolders.ProductFileDownloadFolder, this.appFolders.TempFileDownloadFolder);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Có lỗi: " + ex.Message);
            }
        }

        public async Task<FileDto> DownloadFileUpload(string linkFile)
        {
            if (string.IsNullOrEmpty(linkFile))
            {
                throw new UserFriendlyException(StringResources.NullParameter);
            }

            var fileName = linkFile.Split(Path.DirectorySeparatorChar).Last();
            var path = this.appFolders.ProductFileUploadFolder + linkFile.Replace(fileName, string.Empty);

            // _appFolders.DemoFileDownloadFolder : Thư mục chưa file mẫu cần tải
            // _appFolders.TempFileDownloadFolder : Không được sửa
            return await GlobalFunction.DownloadFileMau(fileName, path, this.appFolders.TempFileDownloadFolder);
        }

        public async Task<FileDto> ExportToExcel(InputProductDto input)
        {
            var list = await this.GetAllAsync(input);
            var time = DateTime.Now.ToString("dd/MM/yyyy");
            using (var package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Product");

                var namedStyle = package.Workbook.Styles.CreateNamedStyle("HyperLink");
                namedStyle.Style.Font.UnderLine = true;
                namedStyle.Style.Font.Color.SetColor(Color.Blue);

                // set header
                worksheet.Cells[1, 1].Value = "Product Name";
                worksheet.Cells[1, 2].Value = "Price";
                worksheet.Cells[1, 3].Value = "Category";
                worksheet.Cells[1, 4].Value = "Description";
                worksheet.Cells[1, 5].Value = "Date Created";
                worksheet.Cells[1, 6].Value = "Last Date Modified";
                worksheet.Cells[1, 7].Value = "User Name";

                // Bôi đậm header
                using (ExcelRange r = worksheet.Cells[1, 1, 1, 7])
                {
                    using (var f = new Font("Calibri", 12))
                    {
                        r.Style.Font.SetFromFont(f);
                        r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                }

                var rowNumber = 2;
                list.Items.ToList().ForEach(item =>
                {
                    worksheet.Cells[rowNumber, 1].Value = item.ProductName;
                    worksheet.Cells[rowNumber, 2].Value = item.Price;
                    worksheet.Cells[rowNumber, 3].Value = item.Category;
                    worksheet.Cells[rowNumber, 4].Value = item.Description;
                    worksheet.Cells[rowNumber, 5].Value = item.DateCreated.ToString("dd-MM-yyyy");
                    worksheet.Cells[rowNumber, 6].Value = item.LastDateModified != null ? item.LastDateModified.Value.ToString("dd-MM-yyyy") : String.Empty;
                    worksheet.Cells[rowNumber, 7].Value = item.UserName;
                    rowNumber++;
                });

                // Cho các ô rộng theo dữ liệu
                worksheet.Cells.AutoFitColumns(0);

                worksheet.PrinterSettings.FitToHeight = 1;

                // Tên file "xlsx"
                var fileName = string.Join(".", new string[] { "Product list_" + time, "xlsx" });

                // Lưu file vào server
                using (var stream = new MemoryStream())
                {
                    package.SaveAs(stream);
                }
                try
                {
                    var file = new FileDto(fileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
                    var filePath = Path.Combine(this.appFolders.TempFileDownloadFolder, file.FileToken);
                    package.SaveAs(new FileInfo(filePath));
                    return file;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }

        public async Task<string> ImportFileExcel(string filePath)
        {
            StringBuilder returnMessage = new StringBuilder();
            returnMessage.Append("Kết quả nhập file:");
            string error = string.Empty;
            filePath = Path.Combine(_env.WebRootPath, filePath);
            //var maxAllow = this.configurationApp.GetValue<int>("Config4App:MaxImport");
            ReadFromExcelDto<CreateInputDto> readResult = new ReadFromExcelDto<CreateInputDto>();

            var productAll = new List<CreateInputDto>();

            IDictionary<int, List<int>> dataCheck = new Dictionary<int, List<int>>();

            IDictionary<int, string> errorMessages = new Dictionary<int, string>();

            errorMessages.Add(1, "Product does exist in the system");
            errorMessages.Add(2, "Product can't be left blank");
            errorMessages.Add(3, "Product exceeds 255 characters");

            foreach (var item in errorMessages)
            {
                dataCheck.Add(item.Key, new List<int>());
            }

            // Không tìm thấy file
            if (!System.IO.File.Exists(filePath))
            {
                readResult.ResultCode = (int)GlobalConst.ReadExcelResultCodeConst.FileNotFound;
            }

            // Đọc hết file excel
            var rowStartRead = 2;
            var data = ReadAndRemoveFileAddNew(filePath, "Import_PRODUCT", rowStartRead);

            //var data = allData.Where(record => record.Any(value => !string.IsNullOrEmpty(value))).ToList();

            // Không có dữ liệu
            if (data.Count <= 0)
            {
                readResult.ResultCode = (int)GlobalConst.ReadExcelResultCodeConst.CantReadData;
                this.Assert(true, "Nội dung file không đúng quy định");
            }
            //else if (data.Count > maxAllow)
            //{
            //    readResult.ResultCode = (int)GlobalConst.ReadExcelResultCodeConst.CantReadData;
            //    this.Assert(true, "Số lượng bản ghi không vượt quá 10.000");
            //}
            //else if (data[0].Count != 5)
            //{
            //    readResult.ResultCode = (int)GlobalConst.ReadExcelResultCodeConst.CantReadData;
            //    this.Assert(true, "Nội dung file không đúng quy định");
            //}
            else
            {
                var productRes = await this.productRepository.GetAll().ToDictionaryAsync(e => e.ProductName, e => e);
                for (int i = 0; i < data.Count; i++)
                {
                    try
                    {
                        string productName = GlobalFunction.RegexFormat(data[i][0]);
                        string price = GlobalFunction.RegexFormat(data[i][1]);
                        string category = GlobalFunction.RegexFormat(data[i][2]);
                        string description = GlobalFunction.RegexFormat(data[i][3]);

                        var isError = false;

                        if (string.IsNullOrEmpty(productName))
                        {
                            isError = true;
                            dataCheck[2].Add(i + 2);
                        }
                        else
                        {
                            var checkProduct = productRes.ContainsKey(productName);
                            var checkProductFile = productAll.Any(e => e.ProductName.Equals(productName));
                            if (productName.Length > 255)
                            {
                                isError = true;
                                dataCheck[3].Add(i + 2);
                            }
                            if (checkProduct || checkProductFile)
                            {
                                isError = true;
                                dataCheck[1].Add(i + 2);
                            }
                        }

                        if (isError)
                        {
                            // Đánh dấu các bản ghi lỗi
                            readResult.ListErrorRow.Add(data[i]);
                            readResult.ListErrorRowIndex.Add(i + 1);
                        }
                        else
                        {
                            var create = new CreateInputDto();
                            create.ProductName = productName.Trim();
                            create.Price = double.Parse(price);
                            create.Category = category;
                            create.Description = description;

                            // Đánh dấu các bản ghi thêm thành công
                            readResult.ListResult.Add(create);
                            productAll.Add(create);
                        }
                        rowStartRead++;
                    }
                    catch (Exception e)
                    {

                        throw e;
                    }
                }
                if (productAll.Count > 0)
                {
                    await this.InsertBulkForAsset(productAll);
                }
            }

            // Thông tin import
            readResult.ErrorMessage = GlobalModel.ReadExcelResultCodeSorted[readResult.ResultCode];

            // Nếu đọc file thất bại
            if (readResult.ResultCode != 200)
            {
                return readResult.ErrorMessage;
            }
            else
            {
                // Đọc file thành công
                // Trả kết quả import
                returnMessage.Append(string.Format("\r\n\u00A0- Total records: {0}", readResult.ListResult.Count + readResult.ListErrorRow.Count));
                returnMessage.Append(string.Format("\r\n\u00A0- Number of successful records: {0}", readResult.ListResult.Count));
                returnMessage.Append(string.Format("\r\n\u00A0- Number of failed records: {0}", readResult.ListErrorRow.Count));
                if (dataCheck.Any(a => a.Value.Any()))
                {
                    foreach (var item in dataCheck.Where(s => s.Value.Any()))
                    {
                        returnMessage.Append("\r\n\u00A0- " + errorMessages[item.Key] + ": Dòng " + string.Join(", ", item.Value));
                    }
                }
            }

            return returnMessage.ToString();
        }

        private List<List<string>> ReadAndRemoveFileAddNew(string FilePath, string SheetName, int startRowIndex = 1)
        {
            List<List<string>> ouput = new List<List<string>>();
            FileInfo fileInfo = new FileInfo(FilePath);
            try
            {
                using (var excelPackage = new ExcelPackage(fileInfo))
                {
                    var sheet = excelPackage.Workbook.Worksheets[SheetName];
                    if (sheet != null)
                    {
                        for (var rowIndex = 0; rowIndex < sheet.Dimension.End.Row - 1; rowIndex++)
                        {
                            List<string> line = new List<string>();
                            for (var colIndex = 0; colIndex < sheet.Dimension.Columns; colIndex++)
                            {
                                var Value = sheet.Cells[rowIndex + startRowIndex, colIndex + 1].Value;
                                line.Add(Value != null ? Value.ToString() : "");
                            }
                            ouput.Add(line);
                        }
                    }
                }
                return ouput;
            }
            catch (Exception exception)
            {
                throw new UserFriendlyException(exception.Message);
            }
        }

        private string GetDatabaseName(string connectionStrings)
        {
            string databaseName;

            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                databaseName = connection.Database;
            }

            return databaseName;
        }

        private async Task InsertBulkForAsset(List<CreateInputDto> productAlls)
        {
            try
            {
                var ghiChuValue = $"Imported_{DateTime.Now:ddMMyyyy_HHmm}";

                if (productAlls.Count() > 0)
                {
                    // Create a DataTable with your data
                    DataTable dataTable = new DataTable();
                    // Add columns to the DataTable 
                    dataTable.Columns.Add("TenantId", typeof(int));
                    dataTable.Columns.Add("Id", typeof(int));
                    dataTable.Columns.Add("ProductName", typeof(string));
                    dataTable.Columns.Add("Price", typeof(double));
                    dataTable.Columns.Add("Category", typeof(string));
                    dataTable.Columns.Add("Description", typeof(string));
                    dataTable.Columns.Add("CreationTime", typeof(DateTime));
                    dataTable.Columns.Add("IsDeleted", typeof(bool));

                    productAlls.ForEach(fc =>
                    {
                        dataTable.Rows.Add(1, fc.Id, fc.ProductName, fc.Price, fc.Category,fc.Description, DateTime.Now, false);
                    });


                    using (SqlConnection connection = new SqlConnection(this.connectionString))
                    {
                        await connection.OpenAsync();

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = this.ProductDatabase;
                            bulkCopy.ColumnMappings.Add("TenantId", "TenantId");
                            bulkCopy.ColumnMappings.Add("ProductName", "ProductName");
                            bulkCopy.ColumnMappings.Add("Price", "Price");
                            bulkCopy.ColumnMappings.Add("Category", "Category");
                            bulkCopy.ColumnMappings.Add("Description", "Description");
                            bulkCopy.ColumnMappings.Add("CreationTime", "CreationTime");
                            bulkCopy.ColumnMappings.Add("IsDeleted", "IsDeleted");


                            bulkCopy.WriteToServer(dataTable);
                        }

                        await connection.CloseAsync();
                    }
                }
                await this.CurrentUnitOfWork.SaveChangesAsync();

            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}