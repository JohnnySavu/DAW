using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialJohnny.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsPrivate { get; set; }

        public virtual ICollection <Post> Posts { get; set; }

    }
}