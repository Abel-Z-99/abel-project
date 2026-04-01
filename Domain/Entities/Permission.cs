using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    /// <summary>
    /// 权限实体（RBAC，如 Product.Read）。
    /// </summary>
    public class Permission
    {
        /// <summary>主键标识。</summary>
        public int Id { get; set; }

        /// <summary>权限编码名称。</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>权限说明。</summary>
        public string? Description { get; set; }

        /// <summary>拥有该权限的角色关联。</summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
