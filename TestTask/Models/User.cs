using Microsoft.AspNetCore.Identity;

namespace TestTask.Models
{
    public class User : IdentityUser
    {
        public ICollection<Catalog>? Catalogs { get; set; }
    }
}
