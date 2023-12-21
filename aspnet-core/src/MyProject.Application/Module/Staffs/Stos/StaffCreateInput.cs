using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DbEntities;

namespace MyProject.DanhMuc.Staffs.Stos
{
    [AutoMap(typeof(Staff))]
    public class StaffCreateInput : EntityDto<int?>
    {
        public string Ma { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        // public List<Staff_File> ListStaffFile { get; set; }
    }
}
