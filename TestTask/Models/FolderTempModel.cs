namespace TestTask.Models
{
    public class FolderTempModel
    {
        public string name;
        public int level;
        public string parent;
        public Guid id;
        public FolderTempModel(string name, int level, string parent, Guid id)
        {
            this.name = name;
            this.level = level;
            this.parent = parent;
            this.id = id;
        }

        public override bool Equals(object? obj)
        {
            return obj is FolderTempModel model &&
                   name == model.name &&
                   parent == model.parent;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode() + parent.GetHashCode();
        }
    }
}
