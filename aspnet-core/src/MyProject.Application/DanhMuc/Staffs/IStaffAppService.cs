using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using MyProject.DanhMuc.Staffs.Stos;
using MyProject.Data;

namespace MyProject.DanhMuc.Staffs
{
    public interface IStaffAppService
    {
        Task<int> CreateOrEdit(StaffCreateInput input);

        Task<StaffCreateInput> GetForEditAsync(EntityDto input);

        Task Delete(EntityDto input);
    }
}
