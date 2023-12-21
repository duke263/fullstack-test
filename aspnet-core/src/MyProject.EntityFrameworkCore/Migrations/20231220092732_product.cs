using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProject.Migrations
{
    public partial class product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Demo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Ma = table.Column<string>(nullable: true),
                    Ten = table.Column<string>(nullable: true),
                    Checkbox = table.Column<string>(nullable: true),
                    CheckboxTrueFalse = table.Column<bool>(nullable: true),
                    RadioButton = table.Column<int>(nullable: true),
                    InputSwitch = table.Column<bool>(nullable: true),
                    InputMask = table.Column<string>(nullable: true),
                    Slider = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    InputTextarea = table.Column<string>(nullable: true),
                    IntegerOnly = table.Column<int>(nullable: true),
                    Decimal = table.Column<double>(nullable: true),
                    DateBasic = table.Column<DateTime>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: true),
                    DateDisable = table.Column<DateTime>(nullable: true),
                    DateMinMax = table.Column<DateTime>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    DateMultiple = table.Column<string>(nullable: true),
                    DateMultipleMonth = table.Column<DateTime>(nullable: true),
                    MonthOnly = table.Column<DateTime>(nullable: true),
                    TimeOnly = table.Column<string>(nullable: true),
                    AutoCompleteSingle = table.Column<int>(nullable: true),
                    AutoCompleteMultiple = table.Column<string>(nullable: true),
                    DropDownSelectTree = table.Column<int>(nullable: true),
                    MutipleCheckboxTree = table.Column<string>(nullable: true),
                    DropdownSingle = table.Column<int>(nullable: true),
                    DropdownMultiple = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Category = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Ma = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreeView",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TreeViewParentId = table.Column<int>(nullable: true),
                    Ma = table.Column<string>(nullable: true),
                    Ten = table.Column<string>(nullable: true),
                    GhiChu = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreeView", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Demo_File",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    DemoId = table.Column<int>(nullable: true),
                    TenFile = table.Column<string>(nullable: true),
                    LinkFile = table.Column<string>(nullable: true),
                    LoaiFile = table.Column<int>(nullable: true),
                    GhiChu = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Demo_File_Demo_DemoId",
                        column: x => x.DemoId,
                        principalTable: "Demo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Demo_File_DemoId",
                table: "Demo_File",
                column: "DemoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Demo_File");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "TreeView");

            migrationBuilder.DropTable(
                name: "Demo");
        }
    }
}
