using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder();

// Load configuration from appsettings.json
builder.Services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) // Set base directory
    .AddJsonFile("src/appsettings.json", optional: false, reloadOnChange: true) // Load JSON config file
    .Build());

// Register HostsFileManager as a singleton service
builder.Services.AddSingleton<HostsFileManager>();

// Build the Cocona command-line app
var app = builder.Build();
app.AddCommands<MainCommands>(); // Add CLI command handlers
app.Run(); // Start the application