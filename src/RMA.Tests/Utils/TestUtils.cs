using System.Text.Json;

namespace RMA.Tests.Utils;

public static class TestUtils
{

    public static async Task<T?> GetJsonPropertyAsync<T>(HttpResponseMessage response, string propertyName)
    {
        if (response.Content == null)
            return default;

        var raw = await response.Content.ReadAsStringAsync();

        using var json = JsonDocument.Parse(raw);

        if (json.RootElement.TryGetProperty(propertyName, out var value))
        {
            try
            {
                return JsonSerializer.Deserialize<T>(value.GetRawText());
            }
            catch
            {
                return default;
            }
        }

        return default;
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