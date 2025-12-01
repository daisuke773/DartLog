using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartLog.Models
{
    [Table("games")]
    public class Game
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // ★ PlayerId → UserId に変更（AspNetUsers.Id と同じ string 型）
        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = null!;

        [Required]
        [Column("played_at")]
        public DateTime PlayedAt { get; set; }

        [Required]
        [Column("total_score")]
        public int TotalScore { get; set; }

        [Column("memo")]
        public string? Memo { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; } = "in_progress";

        [Column("ended_at")]
        public DateTime? EndedAt { get; set; }

        // ★ ナビゲーション：1ゲームは1ユーザー（＝プレイヤー）に属する
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        public List<Throw> Throws { get; set; } = new();
    }
}
