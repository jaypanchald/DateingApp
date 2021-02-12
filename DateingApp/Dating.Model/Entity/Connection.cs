using System.ComponentModel.DataAnnotations;

namespace Dating.Model.Entity
{
    public class Connection
    {
        public Connection()
        { }

        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        [Key]
        public string ConnectionId { get; set; }

        public string Username { get; set; }
    }
}
