using System;
using System.Collections.Generic;
using System.Text;

namespace MyProject.Module.Products.Dtos
{
    public class CreateInputDto
    {
        public int? Id { get; set; }

        public string ProductName { get; set; }

        public string? Description { get; set; }

        public double Price { get; set; }

        public string Category { get; set; }
    }
}
