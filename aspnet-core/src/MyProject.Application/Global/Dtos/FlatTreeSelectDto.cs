﻿// This file is not generated, but this comment is necessary to exclude it from StyleCop analysis 
// <auto-generated/> 
namespace MyProject.Global.Dtos
{
    public class FlatTreeSelectDto : FlatTreeSelectDto<int>
    {
    }

    public class FlatTreeSelectDto<TPrimaryKey>
    {
        public TPrimaryKey Id { get; set; }

        public TPrimaryKey ParentId { get; set; }

        public string DisplayName { get; set; }
    }
}
