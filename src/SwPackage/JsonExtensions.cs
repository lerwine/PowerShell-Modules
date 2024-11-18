using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Json.More;

namespace SwPackage;

public static class JsonExtensions
{
    public static bool TryGetJsonObjectProperty(this JsonObject obj, string propertyName, [NotNullWhen(true)] out JsonObject? result)
    {
        if (obj is not null && obj.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonObject json)
        {
            result = json;
            return true;
        }
        result = null;
        return false;
    }

    public static bool TryGetJsonArrayProperty(this JsonObject obj, string propertyName, [NotNullWhen(true)] out JsonArray? result)
    {
        if (obj is not null && obj.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonArray json)
        {
            result = json;
            return true;
        }
        result = null;
        return false;
    }

    public static bool TryGetJsonValueProperty(this JsonObject obj, string propertyName, [NotNullWhen(true)] out JsonValue? result)
    {
        if (obj is not null && obj.TryGetPropertyValue(propertyName, out JsonNode? node) && node is JsonValue json)
        {
            result = json;
            return true;
        }
        result = null;
        return false;
    }

    public static bool TryGetJsonStringProperty(this JsonObject obj, string propertyName, [NotNullWhen(true)] out string? result)
    {
        if (obj is not null && obj.TryGetJsonValueProperty(propertyName, out JsonValue? node))
        {
            result = node.GetString() ?? node.ToJsonString();
            return true;
        }
        result = null;
        return false;
    }

    public static bool TryGetJsonDateTimeProperty(this JsonObject obj, string propertyName, out DateTime result)
    {
        if (obj is not null && obj.TryGetJsonStringProperty(propertyName, out string? value) && DateTime.TryParse(value, out result))
            return true;
        result = default;
        return false;
    }

    public static bool TryGetJsonBooleanProperty(this JsonObject obj, string propertyName, [NotNullWhen(true)] out bool? result)
    {
        if (obj is not null && obj.TryGetJsonValueProperty(propertyName, out JsonValue? node))
        {
            if (node.GetValueKind() == JsonValueKind.True)
                result = true;
            else if (node.GetValueKind() == JsonValueKind.False)
                result = false;
            else
            {
                result = null;
                return false;
            }
            return true;
        }
        result = null;
        return false;
    }
}