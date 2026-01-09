using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Globalization;

public class WorldTimePlugin
{
    [KernelFunction("GetCityTime")]
    [Description("Get the current time for a specific city")]
    public string GetCityTime([Description("The name of the city, e.g. Tokyo, Zurich, London")] string city)
    {
        try
        {
            // Normalize city name
            city = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(city.Trim().ToLower());

            // Map city names to Windows timezone IDs
            var cityTimezones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Zurich", "Central European Standard Time"},
                {"Geneva", "Central European Standard Time"},
                {"London", "GMT Standard Time"},
                {"New York", "Eastern Standard Time"},
                {"Tokyo", "Tokyo Standard Time"},
                {"Sydney", "AUS Eastern Standard Time"}
            };

            if (!cityTimezones.TryGetValue(city, out string? timezoneId))
                return $"I'm sorry, I don't have the timezone mapping for {city} in my database.";

            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            var cityTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

            return $"It is {cityTime:HH:mm} in {city}.";
        }
        catch (Exception ex)
        {
            return $"Error retrieving time for {city}: {ex.Message}";
        }
    }
}