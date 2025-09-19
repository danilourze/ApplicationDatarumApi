using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace AplicationDatariumApi.Infra.Converters;

public class JsonValueConverter<T> : ValueConverter<T, string>
{
    public JsonValueConverter()
        : base(
            v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = false }),
            v => JsonSerializer.Deserialize<T>(v, new JsonSerializerOptions())!
        )
    { }
}