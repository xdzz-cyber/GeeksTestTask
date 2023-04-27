namespace GeeksTestTask.ViewModels;

public class FolderViewModel
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;

    public List<FolderViewModel> Children { get; set; } = null!;
}