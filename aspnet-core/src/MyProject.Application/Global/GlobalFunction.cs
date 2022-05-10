﻿// This file is not generated, but this comment is necessary to exclude it from StyleCop analysis 
// <auto-generated/> 
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.IO;
using Abp.IO.Extensions;
using Abp.UI;
using DbEntities;
using Microsoft.AspNetCore.Http;
using MyProject.Authorization;
using MyProject.Authorization.Users;
using MyProject.Data;
using MyProject.Global.Dtos;
using MyProject.Net.MimeTypes;
using MyProject.Shared;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyProject.Global
{
    public class GlobalFunction
    {
        public static IAppFolders AppFolders { get; set; }

        public static DateTime? GetDateTime(DateTime? dateTime)
        {
            return dateTime != null ? dateTime.Value.ToLocalTime() : dateTime;
        }

        public static string RegexFormat(string input)
        {
            if (input != null)
            {
                return Regex.Replace(input, @"\s+", " ").Trim();
            }
            else
                return input;
        }

        /// <summary>
        /// Hàm lưu file
        /// </summary>
        /// <param name="FolderPath">Đường dẫn lưu file trên server</param>
        /// <param name="ImportFile">File</param>
        /// <returns>Đường dẫn trỏ tới file trên server</returns>
        public static string SaveFile(string FolderPath, IFormFile ImportFile)
        {
            byte[] fileBytes;
            using (var stream = ImportFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
            }

            string uploadFileName = string.Format("{0:yyyyMMdd_hhmmss}_", DateTime.Now) + ImportFile.FileName;

            // Set full path to upload file
            DirectoryHelper.CreateIfNotExists(FolderPath);
            string uploadFilePath = Path.Combine(FolderPath, uploadFileName);
            // Save new file
            File.WriteAllBytes(uploadFilePath, fileBytes);
            return uploadFilePath;
        }

        /// <summary>
        /// Đọc file excel
        /// </summary>
        /// <param name="FilePath">Đường dẫn file excel cần đọc trên server</param>
        /// <param name="startRowIndex">Đọc từ dòng nào</param>
        /// <returns>Dữ liệu đọc được từ file excel</returns>
        public static async Task<List<List<string>>> ReadFromExcel(string FilePath, int startRowIndex = 2, int sheetIndex = 1)
        {
            List<List<string>> Result = new List<List<string>>();
            FileInfo fileInfo = new FileInfo(FilePath);
            try
            {
                using (var excelPackage = new ExcelPackage(fileInfo))
                {
                    var sheet = excelPackage.Workbook.Worksheets[sheetIndex];

                    if (sheet != null)
                    {
                        for (var rowIndex = 0; rowIndex < sheet.Dimension.End.Row - 1; rowIndex++)
                        {
                            List<string> Line = new List<string>();

                            for (var colIndex = 0; colIndex < sheet.Dimension.Columns; colIndex++)
                            {
                                var Value = sheet.Cells[rowIndex + startRowIndex, colIndex + 1].Value;
                                Line.Add(Value != null ? Value.ToString() : "");
                            }
                            Result.Add(Line);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw new UserFriendlyException(StringResources.FileSaiDinhDang);
            }
            return await Task.FromResult(Result);
        }
        /// <summary>
        /// Tải file mẫu
        /// </summary>
        /// <param name="FileName">Tên file cần tải</param>
        /// <param name="pathFileDownload">Thư mục chứa file cần tải</param>
        /// <param name="pathFileToken">Thư mục chứa token down ( Không được sửa )</param>
        /// <returns></returns>
        public static Task<FileDto> DownloadFileMau(string FileName, string pathFileDownload, string pathFileToken)
        {
            var result = new FileDto(FileName, MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet);
            string SourceFile = Path.Combine(pathFileDownload, FileName);
            string DestinationFile = Path.Combine(pathFileToken, result.FileToken);
            File.Copy(SourceFile, DestinationFile, true);
            return Task.FromResult(result);
        }

        public static string GetDataApi(string url)
        {
            try
            {
                string token = GetTokenSmartA();
                WebRequest httpWebRequest = HttpWebRequest.Create(url);
                httpWebRequest.Headers.Add("Content-type", "text/json");
                httpWebRequest.Headers.Add("Authorization", token);
                WebResponse response = httpWebRequest.GetResponse();

                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                StreamReader reader = streamReader;

                string responseText = reader.ReadToEnd();
                return responseText;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public static string PostDataApi(string url, string json)
        {
            try
            {
                string token = GetTokenSmartA();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization", token);
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public static string GetTokenSmartA()
        {
            string hostNoc = ConfigurationManager.AppSettings["hostNoc"].ToString();
            if (hostNoc != "")
            {
                string url = hostNoc + "/api/TokenAuth/Authenticate";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"userNameOrEmailAddress\":\"adminpkth@mobifone.vn\"," +
                                  "\"password\":\"123456\"}";
                    streamWriter.Write(json);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var json = streamReader.ReadToEnd();
                    dynamic stuff = JsonConvert.DeserializeObject(json);
                    string token = stuff.result.accessToken;
                    return "Bearer " + token;
                }
            }
            return null;
        }

        /// <summary>
        /// Hàm tính công thức đã được setup.
        /// </summary>
        /// <param name="expression">Công thức cần tình</param>
        /// <returns>Trả về kiểu int.</returns>

        public static double Evaluate(string expression)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                expression = expression.Replace(",", ".");
                var listGiaTri = expression.Split(new char[] { '+', '-', '*', '/', '(', ')', '[', ']' }).Where(e => !string.IsNullOrEmpty(e)).Distinct().ToList();
                string pattern = string.Empty;
                string replace = ".0";

                foreach (var item in listGiaTri)
                {
                    if (!item.Contains(".") && item != "0")
                    {
                        pattern = String.Format(@"\b{0}\b", item);
                        expression = Regex.Replace(expression, pattern, item + replace);
                    }
                }

                using (DataTable table = new DataTable())
                {
                    table.Columns.Add("myExpression", typeof(double), expression);
                    DataRow row = table.NewRow();
                    table.Rows.Add(row);
                    string myExpression = row["myExpression"].ToString() ?? "0";
                    return Math.Round(double.Parse(myExpression) > 0 ? double.Parse(myExpression) : 0);
                }
            }
            else
            {
                return 0;
            }
        }

        public static string GetDateStringFormat(DateTime? date,string format= "dd/MM/yyyy")
        {
            return date != null ? date.Value.ToString(format) : string.Empty;
        }

        public static DateTime? ConvertStringToDateTime(string input)
        {
            try
            {
                if (input.Length < 10)
                {
                    var list = input.Split('/');
                    input = list[0].PadLeft(2, '0') + "/" + list[1].PadLeft(2, '0') + "/" + list[2];
                }
                return DateTime.ParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<List<TreeviewItemDto>> GetAllDropDownTreeViewAsync(IRepository<TreeView> treeViewRepository)
        {
            var listParent = await treeViewRepository.GetAllListAsync();
            var listTong = GetLoaiTaiSanChildren(listParent, null);
            return listTong;
        }

        private static List<TreeviewItemDto> GetLoaiTaiSanChildren(List<TreeView> list, int? id)
        {
            return list.Where(w => w.TreeViewParentId == id).Select(w => new TreeviewItemDto
            {
                Text = w.Ten,
                Value = w.Id,
                Checked = false,
                Children = GetLoaiTaiSanChildren(list, w.Id),
            }).ToList();
        }
    }
}
