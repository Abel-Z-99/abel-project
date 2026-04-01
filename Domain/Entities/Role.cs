using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    /// <summary>
    /// 角色实体（RBAC）。
    /// </summary>
    public class Role
    {
        /// <summary>主键标识。</summary>
        public int Id { get; set; }

        /// <summary>角色名称（如 Admin、User）。</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>角色说明。</summary>
        public string? Description { get; set; }

        /// <summary>拥有该角色的用户关联。</summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>该角色拥有的权限关联。</summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
