using System;
using System.IO;
using System.Linq;

var rootPath = args.Any() ? args[0] : Environment.CurrentDirectory;

var root = new DirectoryInfo(rootPath);

var result = root.GetDirectories("bin", SearchOption.AllDirectories).Where(IsAbandonedCshrapProject);

bool IsAbandonedCshrapProject(DirectoryInfo dir)
{
    var subdirs = dir.Parent.GetDirectories();

    return subdirs.Any(d => d.Name == "obj") && !dir.Parent.GetFiles().Any(f => f.Extension == ".csproj");
}

Console.WriteLine("Found the following abandoned C# projects:");

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine(string.Join("\n", result.Select(p => p.Parent.Name)));
Console.ResetColor();

Console.WriteLine();
Console.WriteLine("Continue with deletion? (y/n)");

if(Console.ReadKey().Key != ConsoleKey.Y)
{
    Console.WriteLine("'Y' not pressed. Exitting...");
    return;
}

var deleteAll = false;
Console.WriteLine("(CAREFUL!) Would you like to delete all of these directories? (y/n)");

if(Console.ReadKey().Key != ConsoleKey.Y)
{
    Console.WriteLine("'Y' not pressed. Deleting one by one...");
}
else
{
    deleteAll = true;
}

Console.Clear();
foreach(var dir in result)
{
    if(!deleteAll)
    {
        Console.WriteLine("Are you sure you want to delete the following file? (y/n)");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(dir.Parent.FullName);
        Console.ResetColor();

        if(Console.ReadKey().Key != ConsoleKey.Y)
        {
            Console.WriteLine("'Y' not pressed. Skipping...");
            continue;
        }
    }

    try
    {
        dir.Parent.Delete(true);
    }
    catch
    {
    }
}
