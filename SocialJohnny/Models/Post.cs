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
        [Required (ErrorMessage = "Campul titlu este obligatoriu")]
        [MinLength(5)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Campul Mesaj este obligatoriu")]
        public string Text { get; set; }
        public String Date { get; set; }

        //Fk
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    
    }
    
}