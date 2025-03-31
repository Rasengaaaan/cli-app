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
