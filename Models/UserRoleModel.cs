using Microsoft.AspNetCore.Identity;

namespace ECommerceApp.Models
{
    public class UserRoleModel : IdentityUser
    {
        public List<RoleModel> roles { get; set; }
    }

    public class RoleModel { 
        public string RoleId { get; set; }
        public string Name { get; set; }
        public bool HasRole { get; set; }
    }
}
