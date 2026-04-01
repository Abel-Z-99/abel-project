namespace Domain.Entities
{
    /// <summary>
    /// 用户与角色的多对多中间实体。
    /// </summary>
    public class UserRole
    {
        /// <summary>用户标识。</summary>
        public int UserId { get; set; }

        /// <summary>用户导航属性。</summary>
        public User User { get; set; } = null!;

        /// <summary>角色标识。</summary>
        public int RoleId { get; set; }

        /// <summary>角色导航属性。</summary>
        public Role Role { get; set; } = null!;
    }
}
