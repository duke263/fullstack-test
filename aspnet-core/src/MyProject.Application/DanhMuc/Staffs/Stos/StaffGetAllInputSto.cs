using Abp.Application.Services.Dto;

namespace MyProject.DanhMuc.Staffs.Stos
{
    public class StaffGetAllInputSto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
