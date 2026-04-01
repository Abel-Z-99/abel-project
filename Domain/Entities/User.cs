using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    /// <summary>
    /// 用户实体（含后台与商城用户）。
    /// </summary>
    public class User
    {
        /// <summary>主键标识。</summary>
        public int Id { get; set; }

        /// <summary>登录用户名。</summary>
        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>电子邮箱。</summary>
        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>密码哈希（非明文）。</summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>手机号码。</summary>
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        /// <summary>头像图片地址。</summary>
        [MaxLength(500)]
        public string AvatarUrl { get; set; } = string.Empty;

        /// <summary>角色简写标识（如 Admin、User），与 RBAC 角色表可并存。</summary>
        public string Role { get; set; } = "User";

        /// <summary>账号是否启用。</summary>
        public bool Status { get; set; } = true;

        /// <summary>注册/创建时间（UTC）。</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>用户与角色的多对多关联。</summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
