using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var contentTypeProvider = new FileExtensionContentTypeProvider
{
    Mappings =
    {
        [".dat"] = "application/octet-stream",
        [".dll"] = "application/octet-stream",
        [".abb"] = "application/octet-stream",
        [".apk"] = "application/octet-stream",
        [".flatpak"] = "application/octet-stream",
    },
};

app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = contentTypeProvider });
app.MapGet("/", () => Results.Redirect("/index.html", true));
app.Run();
