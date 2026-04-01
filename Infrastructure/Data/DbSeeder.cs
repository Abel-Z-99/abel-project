using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext dbContext)
        {
            await dbContext.Database.MigrateAsync();

            await EnsureRolesAndPermissionsAsync(dbContext);

            var adminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var userRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");

            if (!await dbContext.Users.AnyAsync(u => u.Username == "admin"))
            {
                var admin = new User
                {
                    Username = "admin",
                    Email = "admin@webshop.local",
                    Phone = "",
                    AvatarUrl = "",
                    PasswordHash = HashPassword("Admin123!"),
                    Role = "Admin",
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.Users.Add(admin);
                await dbContext.SaveChangesAsync();

                if (adminRole != null)
                {
                    dbContext.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = adminRole.Id });
                    await dbContext.SaveChangesAsync();
                }
            }

            if (!await dbContext.Users.AnyAsync(u => u.Username == "manager"))
            {
                var manager = new User
                {
                    Username = "manager",
                    Email = "manager@webshop.local",
                    Phone = "",
                    AvatarUrl = "",
                    PasswordHash = HashPassword("Manager123!"),
                    Role = "User",
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.Users.Add(manager);
                await dbContext.SaveChangesAsync();

                if (userRole != null)
                {
                    dbContext.UserRoles.Add(new UserRole { UserId = manager.Id, RoleId = userRole.Id });
                    await dbContext.SaveChangesAsync();
                }
            }

            if (!await dbContext.Users.AnyAsync(u => u.Username == "demo"))
            {
                var demo = new User
                {
                    Username = "demo",
                    Email = "demo@webshop.local",
                    Phone = "",
                    AvatarUrl = "",
                    PasswordHash = HashPassword("Demo123!"),
                    Role = "User",
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.Users.Add(demo);
                await dbContext.SaveChangesAsync();

                if (userRole != null)
                {
                    dbContext.UserRoles.Add(new UserRole { UserId = demo.Id, RoleId = userRole.Id });
                    await dbContext.SaveChangesAsync();
                }
            }

            if (!await dbContext.Products.AnyAsync())
            {
                dbContext.Products.AddRange(
                    new Product
                    {
                        Name = "iPhone 15",
                        Description = "Apple smartphone 128GB",
                        Price = 5999,
                        Stock = 50,
                        ImageUrl = "",
                        IsOnSale = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Xiaomi Air Fryer",
                        Description = "Smart air fryer for family use",
                        Price = 399,
                        Stock = 120,
                        ImageUrl = "",
                        IsOnSale = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Nike Running Shoes",
                        Description = "Lightweight running shoes",
                        Price = 699,
                        Stock = 80,
                        ImageUrl = "",
                        IsOnSale = true,
                        CreatedAt = DateTime.UtcNow
                    });

                await dbContext.SaveChangesAsync();
            }

            await EnsureSampleOrdersAsync(dbContext);
        }

        /// <summary>
        /// 首次部署时写入若干示例订单（含明细并扣减库存），仅在 <c>Orders</c> 表为空时执行。
        /// </summary>
        private static async Task EnsureSampleOrdersAsync(ApplicationDbContext dbContext)
        {
            if (await dbContext.Orders.AnyAsync())
            {
                return;
            }

            var demo = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == "demo");
            var manager = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == "manager");
            if (demo == null || manager == null)
            {
                return;
            }

            var products = await dbContext.Products.OrderBy(p => p.Id).ToListAsync();
            if (products.Count == 0)
            {
                return;
            }

            static void AddLine(Order order, Product product, int quantity)
            {
                if (quantity <= 0 || product.Stock < quantity)
                {
                    return;
                }

                var unit = product.Price;
                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    UnitPrice = unit,
                    TotalPrice = unit * quantity
                });
                product.Stock -= quantity;
            }

            static void FinalizeOrder(Order order)
            {
                order.TotalAmount = order.Items.Sum(i => i.TotalPrice);
            }

            var p0 = products[0];
            var p1 = products.Count > 1 ? products[1] : products[0];
            var p2 = products.Count > 2 ? products[2] : products[0];

            var orderPending = new Order
            {
                UserId = demo.Id,
                OrderNo = "WS-SEED-0001",
                Status = "Pending",
                ShippingAddress = "上海市浦东新区示例路 1 号",
                ContactName = "张三",
                ContactPhone = "13800000001",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            };
            AddLine(orderPending, p0, 1);
            AddLine(orderPending, p1, products.Count > 1 ? 2 : 0);
            if (orderPending.Items.Count == 0)
            {
                return;
            }

            FinalizeOrder(orderPending);

            var orderPaid = new Order
            {
                UserId = demo.Id,
                OrderNo = "WS-SEED-0002",
                Status = "Paid",
                ShippingAddress = "北京市朝阳区示例大街 88 号",
                ContactName = "李四",
                ContactPhone = "13900000002",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            };
            AddLine(orderPaid, p2, 1);
            if (orderPaid.Items.Count > 0)
            {
                FinalizeOrder(orderPaid);
            }

            var orderShipped = new Order
            {
                UserId = manager.Id,
                OrderNo = "WS-SEED-0003",
                Status = "Shipped",
                ShippingAddress = "广州市天河区示例大道 168 号",
                ContactName = "王五",
                ContactPhone = "13700000003",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            };
            AddLine(orderShipped, p1, 1);
            AddLine(orderShipped, p2, 1);
            if (orderShipped.Items.Count > 0)
            {
                FinalizeOrder(orderShipped);
            }

            var orderCompleted = new Order
            {
                UserId = manager.Id,
                OrderNo = "WS-SEED-0004",
                Status = "Completed",
                ShippingAddress = "深圳市南山区示例科技园 9 栋",
                ContactName = "赵六",
                ContactPhone = "13600000004",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            };
            AddLine(orderCompleted, p0, 1);
            if (orderCompleted.Items.Count > 0)
            {
                FinalizeOrder(orderCompleted);
            }

            dbContext.Orders.Add(orderPending);
            if (orderPaid.Items.Count > 0)
            {
                dbContext.Orders.Add(orderPaid);
            }

            if (orderShipped.Items.Count > 0)
            {
                dbContext.Orders.Add(orderShipped);
            }

            if (orderCompleted.Items.Count > 0)
            {
                dbContext.Orders.Add(orderCompleted);
            }

            await dbContext.SaveChangesAsync();
        }

        private static async Task EnsureRolesAndPermissionsAsync(ApplicationDbContext dbContext)
        {
            var roleAdmin = await EnsureRoleAsync(dbContext, "Admin", "Administrator");
            var roleUser = await EnsureRoleAsync(dbContext, "User", "Regular user");

            var productRead = await EnsurePermissionAsync(dbContext, "Product.Read", "View products");
            var productManage = await EnsurePermissionAsync(dbContext, "Product.Manage", "Manage products");
            var orderRead = await EnsurePermissionAsync(dbContext, "Order.Read", "View orders");
            var orderManage = await EnsurePermissionAsync(dbContext, "Order.Manage", "Manage orders");
            var userManage = await EnsurePermissionAsync(dbContext, "User.Manage", "Manage users");

            await EnsureRolePermissionAsync(dbContext, roleAdmin.Id, productRead.Id);
            await EnsureRolePermissionAsync(dbContext, roleAdmin.Id, productManage.Id);
            await EnsureRolePermissionAsync(dbContext, roleAdmin.Id, orderRead.Id);
            await EnsureRolePermissionAsync(dbContext, roleAdmin.Id, orderManage.Id);
            await EnsureRolePermissionAsync(dbContext, roleAdmin.Id, userManage.Id);
            await EnsureRolePermissionAsync(dbContext, roleUser.Id, productRead.Id);
            await EnsureRolePermissionAsync(dbContext, roleUser.Id, orderRead.Id);
        }

        private static async Task<Role> EnsureRoleAsync(ApplicationDbContext dbContext, string name, string description)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == name);
            if (role != null)
            {
                return role;
            }

            role = new Role { Name = name, Description = description };
            dbContext.Roles.Add(role);
            await dbContext.SaveChangesAsync();
            return role;
        }

        private static async Task<Permission> EnsurePermissionAsync(ApplicationDbContext dbContext, string name, string description)
        {
            var permission = await dbContext.Permissions.FirstOrDefaultAsync(x => x.Name == name);
            if (permission != null)
            {
                return permission;
            }

            permission = new Permission { Name = name, Description = description };
            dbContext.Permissions.Add(permission);
            await dbContext.SaveChangesAsync();
            return permission;
        }

        private static async Task EnsureRolePermissionAsync(ApplicationDbContext dbContext, int roleId, int permissionId)
        {
            var exists = await dbContext.RolePermissions.AnyAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);
            if (exists)
            {
                return;
            }

            dbContext.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
            await dbContext.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

