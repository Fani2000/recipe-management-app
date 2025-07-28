using System.Text.Json;

namespace RMA.Tests.Utils;

public static class TestUtils
{
    
    public static async Task<string?> GetJsonStringPropertyAsync(HttpResponseMessage response, string propertyName)
    {
        if (response.Content == null)
            return null;

        var raw = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(raw);

        return json.RootElement.TryGetProperty(propertyName, out var value)
            ? value.GetString()
            : null;
    }

    public static async Task<List<T>> GetJsonArrayAsync<T>(HttpResponseMessage response, string propertyName)
    {
        var raw = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(raw);

        if (!json.RootElement.TryGetProperty(propertyName, out var arrayProp) || arrayProp.ValueKind != JsonValueKind.Array)
            return new List<T>();

        var list = new List<T>();
        foreach (var element in arrayProp.EnumerateArray())
        {
            var item = JsonSerializer.Deserialize<T>(element.GetRawText());
            if (item != null)
                list.Add(item);
        }

        return list;
    }
    
}