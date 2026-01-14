using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    static async Task Main(string[] args)
    {
        using HttpClient client = new HttpClient();

        Console.WriteLine(@"  
 ░██████                                      ░██████      ░██                                        
 ░██   ░██                                    ░██   ░██     ░██                                        
░██     ░██ ░████████   ░███████  ░████████  ░██         ░████████  ░███████  ░██░████ ░█████████████  
░██     ░██ ░██    ░██ ░██    ░██ ░██    ░██  ░████████     ░██    ░██    ░██ ░███     ░██   ░██   ░██ 
░██     ░██ ░██    ░██ ░█████████ ░██    ░██         ░██    ░██    ░██    ░██ ░██      ░██   ░██   ░██ 
 ░██   ░██  ░███   ░██ ░██        ░██    ░██  ░██   ░██     ░██    ░██    ░██ ░██      ░██   ░██   ░██ 
  ░██████   ░██░█████   ░███████  ░██    ░██   ░██████       ░████  ░███████  ░██      ░██   ░██   ░██ 
            ░██                                                                                        
            ░██                                                                                        
                                
                                      OpenStorm Updater - Version 1.0");

        HttpResponseMessage response = await client.GetAsync("https://getstorm.superiorcommunist.party/api.php");
        string json = await response.Content.ReadAsStringAsync();

        using JsonDocument parsed = JsonDocument.Parse(json);
        JsonElement root = parsed.RootElement;

        if (!root.TryGetProperty("version", out var updateStatusElement))
        {
            throw new Exception("Missing 'version' in API response.");
        }

        string currentDir = Directory.GetCurrentDirectory();
        string currentVersion = File.ReadAllText(Path.Combine(currentDir, "bin\\current_version.txt"));
        Version currentVersionA = Version.Parse(currentVersion);

        Version latestVersionA = Version.Parse(root.GetProperty("version").ToString());

        string apiVersion = File.ReadAllText(Path.Combine(currentDir, "bin\\current_api_version.txt"));
        Version apiVersionA = Version.Parse(apiVersion);

        Version latestApiVersionA = Version.Parse(root.GetProperty("apiVer").ToString());

        /* 
            Patched exists for when Roblox is outdated and OpenStorm is still waiting for Velocity to update their API
        */

        bool patched = root.GetProperty("patched").GetBoolean();


        if (currentVersionA >= latestVersionA && apiVersionA >= latestApiVersionA && !patched)
        {
            Console.WriteLine("[!] OpenStorm is up to date!");
            Console.WriteLine("{!} Press enter to exit");
            Console.ReadLine();

            System.Environment.Exit(1);
        }

        if (patched)
        {
            Console.WriteLine("[!] OpenStorm is currently patched! Please wait for an update");
            Console.WriteLine("[!] dsc.gg/openstorm for update changelogs");
            Console.WriteLine("{!} Press enter to exit");
            Console.ReadLine();

            System.Environment.Exit(1);
        }

        string requestUri2 = AESEncryption.Decrypt(root.GetProperty("L2").ToString(), root.GetProperty("question").ToString()); // injector
       
        if (!Directory.Exists(Path.Combine(currentDir, "Bin")))
        {
            Directory.CreateDirectory(Path.Combine(currentDir, "Bin"));
        }

        Console.WriteLine("[!] Downloading injector.exe..");

        byte[] dataInjector = await client.GetByteArrayAsync(requestUri2);
        await File.WriteAllBytesAsync(Path.Combine("Bin", "injector.exe"), dataInjector);

        Console.WriteLine("[!] Downloaded injector.exe..");



        string stormExe = "https://getstorm.superiorcommunist.party/assets/" + root.GetProperty("version").ToString() +".exe";
        string stormApi = "https://getstorm.superiorcommunist.party/assets/" + root.GetProperty("version").ToString() + ".api.dll";
        string stormDll = "https://getstorm.superiorcommunist.party/assets/" + root.GetProperty("version").ToString() + ".dll";

        Console.WriteLine("[!] Updating Storm...");
        byte[] stormExeA = await client.GetByteArrayAsync(stormExe);
        await File.WriteAllBytesAsync(Path.Combine(currentDir, "Storm.exe"), stormExeA);

        byte[] stormDllA = await client.GetByteArrayAsync(stormDll);
        await File.WriteAllBytesAsync(Path.Combine(currentDir, "Storm.dll"), stormDllA);

        byte[] stormApiA = await client.GetByteArrayAsync(stormApi);
        await File.WriteAllBytesAsync(Path.Combine(currentDir, "StormAPI.dll"), stormApiA);

        File.WriteAllText(Path.Combine(currentDir, "bin\\current_version.txt"), root.GetProperty("version").ToString());
        File.WriteAllText(Path.Combine(currentDir, "bin\\current_api_version.txt"), root.GetProperty("apiVer").ToString());

        Console.WriteLine("[!] Storm has been updated!");
        Console.WriteLine("{!} Press enter to exit");
        Console.ReadLine();

        System.Environment.Exit(1);
    }
}
