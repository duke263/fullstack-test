using System;
using System.Collections.Generic;
using System.Text;

namespace MyProject.Module.Products.Dtos
{
    public class GetAllOutputDto
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string? Description { get; set; }

        public double Price { get; set; }

        public string Category { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? LastDateModified { get; set; }

        public string UserName { get; set; }
    }
}
