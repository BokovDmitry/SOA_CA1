using System.Text.Json;

namespace SOA_CA1;

public static class DataFileSeeder
{
    private static readonly JsonSerializerOptions SerializerOptions =
        new(JsonSerializerOptions.Web) { WriteIndented = true };

    public static async Task SeedFileAsync<T>(string path, Func<Task<T>> fetch)
    {
        if (File.Exists(path))
        {
            return;
        }

        try
        {
            var data = await fetch();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(path, JsonSerializer.Serialize(data, SerializerOptions));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or ArgumentException or IOException)
        {
            Console.Error.WriteLine($"Could not create the data file {path}: {ex.Message}");
        }
    }
}
