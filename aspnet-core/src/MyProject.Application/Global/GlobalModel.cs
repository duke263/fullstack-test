// This file is not generated, but this comment is necessary to exclude it from StyleCop analysis 
// <auto-generated/> 
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProject.Global
{
    public class GlobalModel
    {
        // Trạng thái thực thi query
        public static SortedList<int, string> TrangThaiThucThiQuerySorted = new SortedList<int, string>
        {
            { 1, "Thành công" },
            { 2, "Thất bại"},
        };

        // Trạng thái Hiệu lực
        public static SortedList<int, string> TrangThaiHieuLucSorted = new SortedList<int, string>
        {
            { 0, "Khởi tạo" },
            { 1, "Hiệu lực"},
            { 2, "Hết hiệu lực"},
        };

        // Trạng thái duyệt
        public static SortedList<int, string> TrangThaiDuyetSorted = new SortedList<int, string>
        {
            { 0, "Khởi tạo" },
            { 1, "Chờ duyệt"},
            { 2, "Đã duyệt"},
            { 3, "Không duyệt"},
        };

        // Trạng thái đọc Excel
        public static SortedList<int, string> ReadExcelResultCodeSorted = new SortedList<int, string>
        {
            { 404   , "Có lỗi: Không tìm thấy file!" },
            { 500   , "Không đọc được dữ liệu, sai cấu trúc file hoặc dữ liệu rỗng!" },
            { 200   , "Thành công!" }
        };
    }
}
