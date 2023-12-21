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
            string fileName = string.Format("ProductImport.xlsx");
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
    }
}