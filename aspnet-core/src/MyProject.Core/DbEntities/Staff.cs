namespace DbEntities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Abp.Domain.Entities.Auditing;

    [Table("Staff")]
    public class Staff : FullAuditedEntity
    {
        public string Ma { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        //public virtual ICollection<Staff_File> ListStaffFile { get; set; }
    }
}
