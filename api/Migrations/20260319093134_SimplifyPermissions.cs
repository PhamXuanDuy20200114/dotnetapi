using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleGroupPermissions");

            migrationBuilder.DropTable(
                name: "UserRoleGroups");

            migrationBuilder.DropTable(
                name: "RoleGroups");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Description", "Group", "Name" },
                values: new object[] { "Xem báo cáo", "Reports", "reports.view" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "Group", "Name" },
                values: new object[] { "Xuất báo cáo", "Reports", "reports.export" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleGroups", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoleGroupPermissions",
                columns: table => new
                {
                    RoleGroupId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleGroupPermissions", x => new { x.RoleGroupId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RoleGroupPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleGroupPermissions_RoleGroups_RoleGroupId",
                        column: x => x.RoleGroupId,
                        principalTable: "RoleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRoleGroups",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleGroups", x => new { x.UserId, x.RoleGroupId });
                    table.ForeignKey(
                        name: "FK_UserRoleGroups_RoleGroups_RoleGroupId",
                        column: x => x.RoleGroupId,
                        principalTable: "RoleGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Description", "Group", "Name" },
                values: new object[] { "Xem phân quyền", "Roles", "roles.view" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Description", "Group", "Name" },
                values: new object[] { "Tạo role group", "Roles", "roles.create" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Group", "Name" },
                values: new object[,]
                {
                    { 21, "Sửa role group", "Roles", "roles.update" },
                    { 22, "Xóa role group", "Roles", "roles.delete" },
                    { 23, "Gán role cho user", "Roles", "roles.assign" },
                    { 24, "Xem báo cáo", "Reports", "reports.view" },
                    { 25, "Xuất báo cáo", "Reports", "reports.export" }
                });

            migrationBuilder.InsertData(
                table: "RoleGroups",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Toàn quyền hệ thống", "Admin" },
                    { 2, "Quản lý sản phẩm, đơn hàng, báo cáo", "Quản lý" },
                    { 3, "Xem và cập nhật đơn hàng", "Nhân viên bán hàng" },
                    { 4, "Quản lý sản phẩm và tồn kho", "Quản lý kho" }
                });

            migrationBuilder.InsertData(
                table: "RoleGroupPermissions",
                columns: new[] { "PermissionId", "RoleGroupId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 11, 1 },
                    { 12, 1 },
                    { 13, 1 },
                    { 14, 1 },
                    { 15, 1 },
                    { 16, 1 },
                    { 17, 1 },
                    { 18, 1 },
                    { 19, 1 },
                    { 20, 1 },
                    { 21, 1 },
                    { 22, 1 },
                    { 23, 1 },
                    { 24, 1 },
                    { 25, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 2 },
                    { 5, 2 },
                    { 6, 2 },
                    { 7, 2 },
                    { 8, 2 },
                    { 9, 2 },
                    { 11, 2 },
                    { 12, 2 },
                    { 24, 2 },
                    { 25, 2 },
                    { 1, 3 },
                    { 5, 3 },
                    { 9, 3 },
                    { 10, 3 },
                    { 11, 3 },
                    { 12, 3 },
                    { 1, 4 },
                    { 2, 4 },
                    { 3, 4 },
                    { 4, 4 },
                    { 5, 4 },
                    { 6, 4 },
                    { 7, 4 },
                    { 8, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleGroupPermissions_PermissionId",
                table: "RoleGroupPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleGroups_RoleGroupId",
                table: "UserRoleGroups",
                column: "RoleGroupId");
        }
    }
}
