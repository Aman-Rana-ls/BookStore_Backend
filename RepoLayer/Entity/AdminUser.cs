using RepoLayer.Entity;
using System.Text.Json.Serialization;

public partial class AdminUser
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;

    [JsonIgnore] 
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
