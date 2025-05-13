using HashcatScheduler.HashcatWorker;

Console.WriteLine("Hello, World!");

var hashcatWorker = new HashcatWorker();
var inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input");

do
{
    Console.Clear();
    Console.WriteLine("Enter the hash file name: ");

    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
        continue;

    var filePath = Path.Combine(inputPath, input);
    if (!File.Exists(filePath))
        continue;

    hashcatWorker.Start(filePath);
    Console.ReadKey();

} while (true);