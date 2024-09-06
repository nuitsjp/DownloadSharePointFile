using System.Security;
using Microsoft.SharePoint.Client;

if (args.Length != 4)
{
    Console.WriteLine("Usage: download-sp-file <siteUrl> <relativePath> <outputPath> <username>");
    return;
}

var siteUrl = args[0];
var fileRelativePath = args[1];
var outputPath = args[2];
var username = args[3];

var password = new SecureString();
Console.Write("Enter your password: ");
while (true)
{
    var key = Console.ReadKey(true);
    if (key.Key == ConsoleKey.Enter)
        break;
    password.AppendChar(key.KeyChar);
    Console.Write("*");
}
Console.WriteLine();

try
{
    using var context = new ClientContext(siteUrl);
    context.Credentials = new SharePointOnlineCredentials(username, password);

    var web = context.Web;
    context.Load(web);
    context.ExecuteQuery();

    var file = web.GetFileByServerRelativeUrl(fileRelativePath);
    context.Load(file);
    context.ExecuteQuery();

    ClientResult<Stream> stream = file.OpenBinaryStream();
    context.ExecuteQuery();

    using (var fileStream = stream.Value)
    using (var outputStream = new FileStream(outputPath, FileMode.Create))
    {
        fileStream.CopyTo(outputStream);
    }

    Console.WriteLine($"File downloaded successfully to: {outputPath}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}