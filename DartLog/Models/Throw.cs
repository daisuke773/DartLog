using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartLog.Models
{
    [Table("throws")]
    public class Throw
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }   // throws.id

        [Required]
        [Column("game_id")]
        public int GameId { get; set; }   // FK → games.id

        [Required]
        [Column("round_no")]
        public int RoundNo { get; set; }  // throws.round_no

        [Required]
        [Column("dart_no")]
        public int DartNo { get; set; }   // throws.dart_no (1〜3想定)

        [Required]
        [Column("score")]
        public int Score { get; set; }    // throws.score (0以上想定)

        // ナビゲーションプロパティ
        [ForeignKey(nameof(GameId))]
        public Game Game { get; set; } = null!;
    }
}
