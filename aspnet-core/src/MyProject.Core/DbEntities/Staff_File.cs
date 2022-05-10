namespace DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;

    [Table("Staff_File")]

    public class Staff_File : FullAuditedEntity
    {
        public string StaffId { get; set; }

        public string NameFile { get; set; }

        public string LinkFile { get; set; }

        public int? LoaiFile { get; set; }

        public string GhiChu { get; set; }
    }
}
