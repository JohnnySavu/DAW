using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialJohnny.Models
{
    public class Profile
    {
        public Profile()
        {
            //this.Groups = new HashSet<Group>();
        }

        [Key]
        public int ProfileId {get;set;}

        public bool IsPrivate { get; set; }

        public string Email { get; set; }

        public string Nickname { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string BirthDate { get; set; }
        public string City { get; set;}
        public string Job { get; set; }
        public string Description { get; set; }
        

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection <Group> Groups { get; set; }
        public virtual ICollection <Profile> FriendRequests { get; set; }
        public virtual ICollection <FriendsProfile> Friends { get; set; }
        
    }
}