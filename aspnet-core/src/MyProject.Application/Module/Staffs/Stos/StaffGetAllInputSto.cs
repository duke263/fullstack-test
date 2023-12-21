using Abp.Application.Services.Dto;

namespace MyProject.Module.Staffs.Stos
{
    public class StaffGetAllInputSto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
