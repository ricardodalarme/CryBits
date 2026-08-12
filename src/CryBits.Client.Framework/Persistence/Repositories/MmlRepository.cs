using AssetManagementBase;
using Myra;
using Myra.Graphics2D.UI;

namespace CryBits.Client.Framework.Persistence.Repositories;

public static class MmlRepository
{
    public static Project Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"MML project file not found: {filePath}");

        var xml = File.ReadAllText(filePath);
        var folder = Path.GetDirectoryName(filePath);
        AssetManager assetManager = !string.IsNullOrEmpty(folder)
            ? AssetManager.CreateFileAssetManager(folder)
            : MyraEnvironment.DefaultAssetManager;

        return Project.LoadFromXml(xml, assetManager);
    }

    public static void Save(string filePath, Project project)
    {
        var xml = project.ToXml();
        File.WriteAllText(filePath, xml);
    }
}
