﻿// This file is not generated, but this comment is necessary to exclude it from StyleCop analysis 
// <auto-generated/> 
using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using MyProject.Authorization.Roles;
using MyProject.Authorization.Users;
using MyProject.MultiTenancy;
using DbEntities;

namespace MyProject.EntityFrameworkCore
{
    public class MyProjectDbContext : AbpZeroDbContext<Tenant, Role, User, MyProjectDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Demo_File> Demo_File { get; set; }
        public DbSet<Demo> Demo { get; set; }
        public DbSet<TreeView> TreeView { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Product_File> Product_File { get; set; }
        public MyProjectDbContext(DbContextOptions<MyProjectDbContext> options)
            : base(options)
        {
        }
    }
}
