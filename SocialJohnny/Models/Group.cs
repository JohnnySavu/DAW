using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialJohnny.Models
{
    public class Group
    {
        public Group ()
        {
            //this.Profiles = new HashSet<Profile>();
        }
        [Key]
        public int GroupId { get; set; }
        [Required (ErrorMessage = "Numele este obligatoriu")]
        public string Name { get; set; }
        [Required]
        public bool IsPrivate { get; set; }

        public virtual ICollection <Post> Posts { get; set; }

        public virtual ICollection <Profile> Profiles { get; set; }
    }
}