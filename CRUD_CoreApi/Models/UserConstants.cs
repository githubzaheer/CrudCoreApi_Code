using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Porduct_Management.Models
{
    public class UserConstants
    {
        public static List<User> Users = new List<User>()
        {
            new User() { Username = "jason_admin", EmailAddress = "jason.admin@email.com", Password = "MyPass_w0rd", GivenName = "Jason", Surname = "Bryant", Role = "Administrator" },
            new User() { Username = "elyse_seller", EmailAddress = "elyse.seller@email.com", Password = "MyPass_w0rd", GivenName = "Elyse", Surname = "Lambert", Role = "Seller" },
        };
    }
}
