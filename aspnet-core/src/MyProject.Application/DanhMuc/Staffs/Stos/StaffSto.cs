using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using DbEntities;
using System;

namespace MyProject.DanhMuc.Staffs.Stos
{
    [AutoMap(typeof(Staff))]
    public class StaffSto : EntityDto<int>
    {
        public string Ma { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }
    }
}
