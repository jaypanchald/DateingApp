using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.Model.Entity
{
    public class Like
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }

        public int LikerId { get; set; }

        public int LikeeId { get; set; }

        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}
