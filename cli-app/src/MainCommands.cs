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
}
