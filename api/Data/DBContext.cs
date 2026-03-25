using Microsoft.EntityFrameworkCore;
using Api.Models.User;
using Api.Models.Auth;
using Api.Models.Product;

namespace Api.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPermission>().HasKey(up => new { up.UserId, up.PermissionId });

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1,  Name = "products.view",        Group = "Products",   Description = "Xem sản phẩm" },
                new Permission { Id = 2,  Name = "products.create",      Group = "Products",   Description = "Tạo sản phẩm" },
                new Permission { Id = 3,  Name = "products.update",      Group = "Products",   Description = "Sửa sản phẩm" },
                new Permission { Id = 4,  Name = "products.delete",      Group = "Products",   Description = "Xóa sản phẩm" },
                new Permission { Id = 5,  Name = "categories.view",      Group = "Categories", Description = "Xem danh mục" },
                new Permission { Id = 6,  Name = "categories.create",    Group = "Categories", Description = "Tạo danh mục" },
                new Permission { Id = 7,  Name = "categories.update",    Group = "Categories", Description = "Sửa danh mục" },
                new Permission { Id = 8,  Name = "categories.delete",    Group = "Categories", Description = "Xóa danh mục" },
                new Permission { Id = 9,  Name = "orders.view",          Group = "Orders",     Description = "Xem đơn hàng" },
                new Permission { Id = 10, Name = "orders.create",        Group = "Orders",     Description = "Tạo đơn hàng" },
                new Permission { Id = 11, Name = "orders.update_status", Group = "Orders",     Description = "Cập nhật trạng thái đơn" },
                new Permission { Id = 12, Name = "orders.cancel",        Group = "Orders",     Description = "Hủy đơn hàng" },
                new Permission { Id = 13, Name = "orders.delete",        Group = "Orders",     Description = "Xóa đơn hàng" },
                new Permission { Id = 14, Name = "users.view",           Group = "Users",      Description = "Xem tài khoản" },
                new Permission { Id = 15, Name = "users.create",         Group = "Users",      Description = "Tạo tài khoản" },
                new Permission { Id = 16, Name = "users.update",         Group = "Users",      Description = "Sửa tài khoản" },
                new Permission { Id = 17, Name = "users.delete",         Group = "Users",      Description = "Xóa tài khoản" },
                new Permission { Id = 18, Name = "users.ban",            Group = "Users",      Description = "Khóa tài khoản" },
                new Permission { Id = 19, Name = "reports.view",         Group = "Reports",    Description = "Xem báo cáo" },
                new Permission { Id = 20, Name = "reports.export",       Group = "Reports",    Description = "Xuất báo cáo" }
            );
        }
    }
}
