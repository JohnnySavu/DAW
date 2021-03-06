﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialJohnny.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public String Text { get; set; }
        public String Date { get; set; }
        public String OwnerNickname { get; set; }

        //FK
        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}