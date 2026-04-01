namespace Domain.Entities
{
    /// <summary>
    /// 角色与权限的多对多中间实体。
    /// </summary>
    public class RolePermission
    {
        /// <summary>角色标识。</summary>
        public int RoleId { get; set; }

        /// <summary>角色导航属性。</summary>
        public Role Role { get; set; } = null!;

        /// <summary>权限标识。</summary>
        public int PermissionId { get; set; }

        /// <summary>权限导航属性。</summary>
        public Permission Permission { get; set; } = null!;
    }
}
