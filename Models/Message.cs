using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class Message : BaseEntity
    {
        [Key]
        public int MessageId { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public List<Comment> Comments { get; set; }

        public Message()
        {
            Comments = new List<Comment>();
        }
    }
}