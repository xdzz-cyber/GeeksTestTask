using GeeksTestTask.DLL;
using GeeksTestTask.Models;
using GeeksTestTask.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GeeksTestTask.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(Guid? id)
    {
        // Get the root directories (i.e. directories without a parent) depending on whether id is null or not
        var folders = id.HasValue
            ? _context.Folders.Include(ch => ch.Children).Where(f => f.Id == id).ToList()
            : _context.Folders.Include(ch => ch.Children).Where(f => f.ParentId == null).ToList();

        // Create a list of directory view models
        var foldersViewModels = folders.Select(f => new FolderViewModel()
        {
            Id = f.Id,
            Name = f.Name,
            Children = GetChildDirectories(f)
        }).ToList();

        // Return the view with the directory view models
        return View(foldersViewModels);
    }

    // Recursive function to get child directories
    private List<FolderViewModel> GetChildDirectories(Folder directory)
    {
        if (directory.Children == null || directory.Children.Count == 0)
        {
            return new List<FolderViewModel>();
        }

        return directory.Children.Select(d => new FolderViewModel()
        {
            Id = d.Id,
            Name = d.Name,
            Children = GetChildDirectories(d)
        }).ToList();
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile importFile)
    {
        //Firstly export current folders state in DB into json file
        Export();

        if (importFile.Length == 0)
        {
            return BadRequest("Please select a file to import.");
        }

        // Read the contents of the file
        using (var stream = new MemoryStream())
        {
            await importFile.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(stream))
            {
                var contents = await reader.ReadToEndAsync();

                // Parse the directory structure from the file contents
                var folders = ParseDirectoryStructure(contents);

                // Save the directory structure to the database
                foreach (var folder in folders) _context.Folders.Add(folder);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }
    }

    // Assuming it is a JSON string which has the following format:
    // [ Id: "94a12fe6-8029-44c5-9430-ac7bbde6349d", Name: "Root Folder", ParentId: "ad803ae3-0ad7-4e1d-8e73-9e0fdc91970a" ]
    private static List<Folder> ParseDirectoryStructure(string contents)

    {
        try
        {
            var folders = new List<Folder>();

            // Parse the JSON string into a list of folders
            var json = JArray.Parse(contents);

            foreach (var item in json)
            {
                var folder = new Folder
                {
                    Id = Guid.Parse(item.Value<string>("Id") ?? throw new InvalidOperationException()),
                    Name = item.Value<string>("Name") ?? throw new InvalidOperationException()
                };

                if (Guid.TryParse(item.Value<string?>("ParentId"), out var parentId)) folder.ParentId = parentId;

                folders.Add(folder);
            }

            // Set up the parent-child relationships
            foreach (var folder in folders)
                if (folder.ParentId != null)
                {
                    var parent = folders.FirstOrDefault(f => f.Id == folder.ParentId);

                    if (parent != null)
                    {
                        folder.Parent = parent;

                        parent.Children = new List<Folder>();

                        parent.Children.Add(folder);
                    }
                }

            return folders.ToList();
        }
        catch (InvalidOperationException invalidOperationException)
        {
            throw new InvalidOperationException(invalidOperationException.Message);
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message);
        }
    }

    private void Export()
    {
        // Get the  directories (i.e. directories without a parent)
        var folders = _context.Folders.Include(ch => ch.Children).ToList();

        // Convert the view models to JSON
        var json = JsonConvert.SerializeObject(folders.Select(f => new Folder()
        {
            Id = f.Id,
            Name = f.Name,
            ParentId = f.ParentId
        }));

        // Write the JSON to a file
        System.IO.File.WriteAllText("directory_structure.json", json);
    }
}