using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DbEntities
{
    [Table("Products")]

    public class Products : FullAuditedEntity, IMayHaveTenant
    {
        public virtual int? TenantId { get; set; }

        public virtual string ProductName { get; set; }

        public virtual string? Description { get; set; }

        public virtual double Price { get; set; }

        public virtual string Category { get; set; }
    }
}
