namespace GeeksTestTask.Models;

public class Folder
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public virtual Folder Parent { get; set; } = null!;
    public virtual ICollection<Folder> Children { get; set; } = null!;
}