// See https://aka.ms/new-console-template for more information

using CertCheck;

Console.WriteLine("Hello, World!");


/* Will accept either a hostname --host, or a plain text file with domain per line */

if (args.Length != 1)
{
    Console.WriteLine("Specify hostname with --host=<host>, or file with domains with --file=<path to file>");
    Environment.Exit(1);
}

var cvs = new CertificateValidationService();

if (args[0].Contains("--host="))
{
    var host = args[0]["--host=".Length..];
    if (!host.StartsWith("https://"))
    {
        Console.Error.WriteLine("Host must be using https.");
        Environment.Exit(1);
    }
    
    Console.WriteLine($"Invoking for host {host}");
    var daysToExpiration = await cvs.GetDaysToExpiration(host);
    Console.WriteLine($"Host {host} will expire in {daysToExpiration} days");
}
else if (args[0].Contains("--file="))
{
    var file = args[0]["--file=".Length..];
    Console.WriteLine($"Invoking for file {file}");

    var hosts = File.ReadAllLines(file);

    foreach (var h in hosts)
    {
        var daysToExpiration = await cvs.GetDaysToExpiration(h);
        Console.WriteLine($"Host {h} will expire in {daysToExpiration} days");
    }
}


Console.ReadKey();