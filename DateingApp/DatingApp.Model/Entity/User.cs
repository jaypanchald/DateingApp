using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.Model.Entity
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [StringLength(50)]
        public string UserName { get; set; }
        
        [StringLength(250)]
        public byte[] PasswordHash { get; set; }
        
        [StringLength(250)]
        public byte[] PasswordSalt { get; set; }
    }
}
