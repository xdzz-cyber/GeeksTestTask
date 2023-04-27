using GeeksTestTask.Models;

namespace GeeksTestTask.DLL;

public class CustomInitializer
{
    private readonly ApplicationDbContext _dbContext;

    public CustomInitializer(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Initialize()
    {
        _dbContext.Database.EnsureCreated();

        // Check if the database has already been seeded
        if (_dbContext.Folders.Any())
        {
            return;
        }

        // Create the root folder
        var rootFolder = new Folder
        {
            Id = Guid.NewGuid(),
            Name = "Root Folder",
            Children = new List<Folder>()
        };

        // Add four child folders to the root folder
        for (int i = 0; i < 4; i++)
        {
            var childFolder1 = new Folder
            {
                Id = Guid.NewGuid(),
                Name = $"Child Folder {i + 1}",
                Parent = rootFolder,
                ParentId = rootFolder.Id,
                Children = new List<Folder>()
            };

            rootFolder.Children.Add(childFolder1);

            // Add three child folders to each child folder of the root folder
            for (int j = 0; j < 3; j++)
            {
                var childFolder2 = new Folder
                {
                    Id = Guid.NewGuid(),
                    Name = $"Child Folder {i + 1}.{j + 1}",
                    Parent = childFolder1,
                    ParentId = childFolder1.Id,
                    Children = new List<Folder>()
                };

                childFolder1.Children.Add(childFolder2);

                // Add two child folders to each child folder of the second level
                for (int k = 0; k < 2; k++)
                {
                    var childFolder3 = new Folder
                    {
                        Id = Guid.NewGuid(),
                        Name = $"Child Folder {i + 1}.{j + 1}.{k + 1}",
                        Parent = childFolder2,
                        ParentId = childFolder2.Id,
                        Children = new List<Folder>()
                    };

                    childFolder2.Children.Add(childFolder3);
                }
            }
        }

        // Save the root folder and its children to the database
        _dbContext.Folders.Add(rootFolder);
        _dbContext.SaveChanges();
    }
}
