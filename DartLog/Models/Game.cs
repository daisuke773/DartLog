using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace DartLog.Models
{
    [Table("games")]
    public class Game
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("player_id")]
        public int PlayerId { get; set; }

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

        // ナビゲーションプロパティ
        [ForeignKey(nameof(PlayerId))]
        public Player Player { get; set; } = null!;   // 1件のGameは1人のPlayerに属する

        public List<Throw> Throws { get; set; } = new(); // 1件のGameに複数Throw
    }
}