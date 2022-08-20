using System.Text.Json;
using TestTask.Data;
using TestTask.Models;

namespace TestTask.Services
{
    public class UploadService : IUploadService
    {
        ApplicationDbContext context = new ApplicationDbContext();

        public async Task Upload(string json, string UserID)
        {
            var files = JsonSerializer.Deserialize<List<string>>(json);
            for (int i = 0; i < files.Count; i++)
            {
                files[i] = files[i].Substring(0, files[i].LastIndexOf('/'));
            }
            files = files.Distinct().ToList();


            List<FolderTempModel> folds = new List<FolderTempModel>();
            foreach (var file in files)
            {
                var folders = file.Split('/').ToList();
                folds.Add(new FolderTempModel(folders[0] + "/", 0, "null", Guid.NewGuid()));
                for (int i = 1; i < folders.Count; i++)
                {
                    string str = "";
                    folders.GetRange(0, i).ForEach(x => str += x + "/");
                    folds.Add(new FolderTempModel(str + folders[i] + "/", i, str, Guid.NewGuid()));
                }
            }
            folds = folds.Distinct().ToList();



            List<CatalogWithParent> catalogs = new List<CatalogWithParent>();
            foreach (var lvl in folds.Select(x => x.level))
            {
                var foldsByLvl = folds.Where(x => x.level == lvl).ToList();
                var foldsParentsByLvl = folds.Where(x => x.level == lvl - 1).ToList();
                foreach (var foldByLvl in foldsByLvl)
                {
                    try
                    {
                        var parent = foldsParentsByLvl.FirstOrDefault(x => x.name == foldByLvl.parent);
                        CatalogWithParent curCat = new()
                        {
                            UserID = UserID,
                            Name = foldByLvl.name,
                            Id = foldByLvl.id,
                            ParentName = parent?.name
                        };
                        curCat.ParentID = parent == null ? new Guid("00000000-0000-0000-0000-000000000000") : parent.id;
                        if (!catalogs.Any(x => x.ParentName + x.Name == curCat.ParentName + curCat.Name))
                            catalogs.Add(curCat);
                    }
                    catch { }
                }
            }


            foreach (var item in catalogs)
            {
                string nm = item.Name.Contains('/') ?
                                item.Name.Remove(item.Name.Length - 1).Contains('/') ?
                                    item.Name.Remove(item.Name.Length - 1).Substring(item.Name.Remove(item.Name.Length - 1).LastIndexOf('/') + 1) :
                                    item.Name.Remove(item.Name.Length - 1)
                            : item.Name;
                context.Catalogs.Add(new Catalog()
                {
                    id = item.Id,
                    Name = nm,
                    ParentID = item.ParentID,
                    UserID = item.UserID
                });
            }
            await context.SaveChangesAsync();
        }
    }
}
