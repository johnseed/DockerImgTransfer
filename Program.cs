﻿// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using X.Common.Helper;

var output = CommandHelper.Execute("docker", "image ls");
var images = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x =>
{
    var split = x.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
    return new Image(split[0], split[1], split[2]);
}).ToList();
int count = 0;
foreach (var image in images)
    Console.WriteLine($"{count++,-2} : {image.Repo,-35}{image.Tag,-20}\t{image.Id,-20}");

Console.WriteLine("Select images to export, use space to seperate");
string? input = Console.ReadLine();
if(string.IsNullOrWhiteSpace(input))
    input = string.Join(" ", Enumerable.Range(0, images.Count).Select(x => x.ToString()));
var selected = Regex.Split(input, @"\s+").Select(x => int.Parse(x)).ToArray();
foreach (var index in selected)
{
    var image = images[index];
    string name = $"{image.Repo}-{image.Tag}";
    Console.WriteLine($"Exporting {image.Repo}:{image.Tag}");
    CommandHelper.Execute("docker", $"save {image.Repo}:{image.Tag} -o {image.Repo}-{image.Tag}.tar");
    Console.WriteLine($"Exported {image.Repo}:{image.Tag}. Compressing...");
    string compressResult = CommandHelper.Execute("docker", $"run --rm -v {Environment.CurrentDirectory}:/app 7z a -mx9 {name}.7z {name}.tar");
    Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
    Console.WriteLine(compressResult);
    Console.WriteLine("Compressed");
    FileInfo file = new FileInfo($"{name}.tar");
    if (file.Exists) file.Delete();
}

Console.WriteLine("Hello, World!");

public class Image
{
    public Image(string repo, string tag, string id)
    {
        Repo = repo;
        Tag = tag;
        Id = id;
    }

    public string Repo { get; set; }
    public string Tag { get; set; }
    public string Id { get; set; }
}