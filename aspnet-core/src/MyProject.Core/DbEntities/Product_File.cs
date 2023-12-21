using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbEntities
{
    public class Product_File : FullAuditedEntity, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }

        public string ProductId { get; set; }

        public string NameFile { get; set; }

        public string LinkFile { get; set; }

        public int? LoaiFile { get; set; }

        public string GhiChu { get; set; }
    }
}
