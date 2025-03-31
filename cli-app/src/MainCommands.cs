using Cocona;
using Microsoft.Extensions.Configuration;

class MainCommands
{
    private readonly HostsFileManager _hostsFileManager;

    // Constructor: Initializes HostsFileManager with configuration
    public MainCommands(IConfiguration config)
    {
        _hostsFileManager = new HostsFileManager(config);
    }

    // CLI command: Sets a new host entry in the HOSTS file
    [Command("set")]
    public void Set([Option("ip")] string ip, [Option('a')] string hostname)
    {
        _hostsFileManager.UpdateHostEntry(ip, hostname);
    }

    // CLI command: Removes a host entry from the HOSTS file
    [Command("remove")]
    public void Remove([Option('a')] string hostname)
    {
        _hostsFileManager.RemoveHostEntry(hostname);
    }

     // CLI command: Restores a specific backup or the latest one if no path is provided
    [Command("restore")]
    public void Restore([Option('p')] string? backupFilePath = null)
    {
        _hostsFileManager.RestoreBackup(backupFilePath);
    }
}
