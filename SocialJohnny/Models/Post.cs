using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SocialJohnny.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public String Date { get; set; }

        //Fk
        public int GroupId { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
    
    }
    
}