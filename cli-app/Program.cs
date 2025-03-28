using Cocona;
using System;
using System.IO;
using System.Linq;

CoconaApp.Run<MainCommands>();

class MainCommands
{
    private const string BaseFilePath = @"C:\Windows\System32\drivers\etc\HOSTS";
    private const string BackupFolder = @"C:\Windows\System32\drivers\etc\backups"; // Backup folder

    [Command("set")]
    public void Set([Option("ip")] string ip, [Option('a')] string hostname)
    {
        string newEntry = $"{ip} {hostname}";
        string[] existingLines = File.Exists(BaseFilePath) ? File.ReadAllLines(BaseFilePath) : Array.Empty<string>();

        bool hostnameExists = false;
        bool changesMade = false;

        for (int i = 0; i < existingLines.Length; i++)
        {
            if (existingLines[i].EndsWith(" " + hostname)) // Change condition to match hostname instead of IP
            {
                if (existingLines[i] != newEntry)
                {
                    existingLines[i] = newEntry;
                    changesMade = true;
                }
                hostnameExists = true;
                break;
            }
        }

        if (!hostnameExists)
        {
            existingLines = existingLines.Append(newEntry).ToArray();
            changesMade = true;
        }

        if (changesMade)
        {
            CreateBackupFile();
            File.WriteAllLines(BaseFilePath, existingLines);
            Console.WriteLine("HOSTS file updated!");
        }
        else
        {
            Console.WriteLine("No changes detected. HOSTS file remains the same.");
        }
        Console.WriteLine(File.ReadAllText(BaseFilePath));
    }

    private void CreateBackupFile()
    {
        if (!File.Exists(BaseFilePath)) return;

        Directory.CreateDirectory(BackupFolder);

        int version = 1;
        string backupFilePath;

        do
        {
            backupFilePath = Path.Combine(BackupFolder, $"HOSTS_{version}.txt");
            version++;
        } while (File.Exists(backupFilePath));

        File.Copy(BaseFilePath, backupFilePath);
        Console.WriteLine($"Backup created: {backupFilePath}");
    }
}