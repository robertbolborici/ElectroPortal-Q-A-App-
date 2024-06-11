using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ElectroPortal.Models
{
    public class User : IdentityUser
    {
        public ICollection<Question> Questions { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<UserReward> UserRewards { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }

        public bool IsFirstNameVisible { get; set; }
        public bool IsLastNameVisible { get; set; }
        public bool IsCountryVisible { get; set; }
    }
}
