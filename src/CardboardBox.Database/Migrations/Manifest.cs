using System.Text.Json.Serialization;

namespace CardboardBox.Database.Migrations;

internal class Manifest
{
    /// <summary>
    /// The directory to execute the migration from
    /// </summary>
    [JsonPropertyName("workDir")]
    public string WorkDir { get; set; } = string.Empty;

    /// <summary>
    /// The available scripts to execute
    /// </summary>
    [JsonPropertyName("paths")]
    public string[] Paths { get; set; } = Array.Empty<string>();
}
