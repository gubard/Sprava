using Gaia.Helpers;

var folders = Enum.GetValues<Environment.SpecialFolder>();

Console.WriteLine($"OS: {OsHelper.OsType}");

foreach (var folder in folders)
{
    Console.WriteLine($"{folder}: {folder.GetPath()}");
}
