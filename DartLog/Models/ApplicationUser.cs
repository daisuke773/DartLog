using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartLog.Models
{
    public class ApplicationUser : IdentityUser
    {
        // プレイヤー名として使う（必要なら）
        [PersonalData]
        [Column("display_name")]
        public string? DisplayName { get; set; }

        [PersonalData]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 1ユーザーが複数ゲームを持つ
        public List<Game> Games { get; set; } = new();
    }
}
