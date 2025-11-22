using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartLog.Models
{
    [Table("players")]
    public class Player
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }   // players.id

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;   // players.name

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // or DateTime.Now

        // ナビゲーションプロパティ（1人のプレイヤーが複数Gameを持つ）
        public List<Game> Games { get; set; } = new();
    }
}
