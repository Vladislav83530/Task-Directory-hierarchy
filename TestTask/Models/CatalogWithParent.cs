namespace TestTask.Models
{
    public class CatalogWithParent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ParentName { get; set; }
        public Guid ParentID { get; set; }
        public string? UserID { get; set; }
    }
}
