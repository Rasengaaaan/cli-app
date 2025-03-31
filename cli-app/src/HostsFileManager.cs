using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

public class HostsFileManager
{
    private readonly string _hostsFilePath;
    private readonly string _backupFolderPath;

    // Constructor: Reads configuration from appsettings.json
    public HostsFileManager(IConfiguration config)
    {
        _hostsFilePath = config["HostsFilePath"];
        _backupFolderPath = config["BackupFolderPath"];
    }

    // Method to update or add a host entry in the HOSTS file
    public void UpdateHostEntry(string ip, string hostname)
    {
        string newEntry = $"{ip} {hostname}";
        string[] existingLines = File.Exists(_hostsFilePath) ? File.ReadAllLines(_hostsFilePath) : Array.Empty<string>();

        bool hostnameExists = false;
        bool changesMade = false;

        // Loop through each line to check if the hostname already exists
        for (int i = 0; i < existingLines.Length; i++)
        {
            // If entry exists but has a different IP, update it
            if (existingLines[i].EndsWith(" " + hostname))
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

        // If the hostname was not found, append the new entry
        if (!hostnameExists)
        {
            existingLines = existingLines.Append(newEntry).ToArray();
            changesMade = true;
        }

        // If changes were made, create a backup and update the file
        if (changesMade)
        {
            CreateBackupFile();
            File.WriteAllLines(_hostsFilePath, existingLines);
            Console.WriteLine("HOSTS file updated!");
        }
        else
        {
            Console.WriteLine("No changes detected.");
        }
    }

// Method to remove a host entry from the HOSTS file
    public void RemoveHostEntry(string hostname)
    {
        if (!File.Exists(_hostsFilePath))
        {
            Console.WriteLine("HOSTS file not found.");
            return;
        }

        string[] existingLines = File.ReadAllLines(_hostsFilePath);
        string[] updatedLines = existingLines.Where(line => !line.EndsWith(" " + hostname)).ToArray();

        if (existingLines.Length == updatedLines.Length)
        {
            Console.WriteLine("No matching entry found to remove.");
            return;
        }

        CreateBackupFile();
        File.WriteAllLines(_hostsFilePath, updatedLines);
        Console.WriteLine("HOSTS entry removed successfully.");
    }
    
 // Method to restore a specific or latest backup of the HOSTS file
    public void RestoreBackup(string? backupFilePath)
    {
        if (!string.IsNullOrEmpty(backupFilePath))
        {
            string resolvedBackupPath = Path.IsPathRooted(backupFilePath) 
                ? backupFilePath 
                : Path.Combine(_backupFolderPath, backupFilePath);
            
            if (!File.Exists(resolvedBackupPath))
            {
                Console.WriteLine($"Specified backup file not found: {resolvedBackupPath}");
                return;
            }
            File.Copy(resolvedBackupPath, _hostsFilePath, true);
            Console.WriteLine($"HOSTS file restored from specified backup: {resolvedBackupPath}");
        }
        else
        {
            if (!Directory.Exists(_backupFolderPath))
            {
                Console.WriteLine("No backup folder found.");
                return;
            }

            var backupFiles = Directory.GetFiles(_backupFolderPath, "HOSTS_*.txt")
                                       .OrderByDescending(File.GetCreationTime)
                                       .ToList();

            if (!backupFiles.Any())
            {
                Console.WriteLine("No backup files found.");
                return;
            }

            string latestBackup = backupFiles.First();
            File.Copy(latestBackup, _hostsFilePath, true);
            Console.WriteLine($"HOSTS file restored from latest backup: {latestBackup}");
        }
    }

    // Method to create a backup of the existing HOSTS file before modification
    private void CreateBackupFile()
    {
        if (!File.Exists(_hostsFilePath)) return; // If the HOSTS file doesn't exist, exit

        Directory.CreateDirectory(_backupFolderPath); // Ensure the backup directory exists
        int version = 1;
        string backupFilePath;

        // Find the next available backup file name (HOSTS_1.txt, HOSTS_2.txt, etc.)
        do
        {
            backupFilePath = Path.Combine(_backupFolderPath, $"HOSTS_{version}.txt");
            version++;
        } while (File.Exists(backupFilePath));

        // Copy the existing HOSTS file to the backup location
        File.Copy(_hostsFilePath, backupFilePath);
        Console.WriteLine($"Backup created: {backupFilePath}");
    }
}
