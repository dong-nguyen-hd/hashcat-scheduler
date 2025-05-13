using System.Diagnostics;
using System.Text.Json;

namespace HashcatScheduler.HashcatWorker;

public class HashcatWorker
{
    #region Properties

    private readonly Dictionary<string, string> _dictionaries = GetDictionaries();
    private readonly SortedDictionary<int, string> _directions = GetDirections();
    private readonly string _hashcatPath = GetHashcatPath();

    #endregion

    #region Method

    public void Start(string inputPath)
    {
        foreach (var process in Process.GetProcessesByName("hashcat"))
            process.Kill();

        string currentDirection = string.Empty;
        foreach (var direction in _directions)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Direction: {direction}\n");
            Console.ResetColor();

            var tempDirection = MappingSection(direction.Value, _dictionaries, inputPath);
            currentDirection = tempDirection;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(_hashcatPath, "hashcat.exe"),
                    WorkingDirectory = _hashcatPath,
                    Arguments = tempDirection,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };

            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += OnProcessOnErrorDataReceived;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode == 0) // Is cracked
                break;
        }

        var resultProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(_hashcatPath, "hashcat.exe"),
                WorkingDirectory = _hashcatPath,
                Arguments = $"--show {currentDirection}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            }
        };

        resultProcess.Start();
        string output = resultProcess.StandardOutput.ReadToEnd();
        resultProcess.WaitForExit();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Finished!");
        Console.WriteLine("Cracked hashes:");
        Console.WriteLine(output);
        Console.ResetColor();
    }

    #region Private work

    private void OnProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data))
            return;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[err] {e.Data}");
        Console.ResetColor();
    }

    private static string MappingSection(string direction, Dictionary<string, string> dictionaries, string inputPath)
    {
        string tempDirection = direction;

        tempDirection = tempDirection.Replace("[input]", $"\"{inputPath}\"");

        var dictionariesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dictionaries");
        foreach (var dictionary in dictionaries)
            tempDirection = tempDirection.Replace($"[{dictionary.Key}]", $"\"{Path.Combine(dictionariesPath, dictionary.Value)}\"");

        return tempDirection;
    }

    private static Dictionary<string, string> GetDictionaries()
    {
        var dictionariesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dictionaries");
        if (!Directory.Exists(dictionariesPath))
            throw new ArgumentNullException($"Path not found: {dictionariesPath}");

        var mappingPath = Path.Combine(dictionariesPath, "mapping.json");

        var jsonContent = File.ReadAllText(mappingPath);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent) ?? throw new ArgumentNullException();
    }

    private static SortedDictionary<int, string> GetDirections()
    {
        var directionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "directions");
        if (!Directory.Exists(directionsPath))
            throw new ArgumentNullException($"Path not found: {directionsPath}");

        var result = new SortedDictionary<int, string>();
        var mappingPath = Path.Combine(directionsPath, "directions.json");

        var jsonContent = File.ReadAllText(mappingPath);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent) ?? throw new ArgumentNullException();

        foreach (var kvp in dictionary)
            if (int.TryParse(kvp.Key, out int key))
                result[key] = kvp.Value;

        return result;
    }

    private static string GetHashcatPath()
    {
        var one = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashcat");
        if (!Directory.Exists(one))
            throw new ArgumentNullException($"Path not found: {one}");

        string pattern = "hashcat";
        var two = Directory
            .EnumerateDirectories(one)
            .FirstOrDefault(dir => new DirectoryInfo(dir).Name.StartsWith(pattern));
        if (string.IsNullOrEmpty(two))
            throw new ArgumentNullException($"No hashcat directory found");

        return two;
    }

    #endregion

    #endregion
}