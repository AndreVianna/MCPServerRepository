using System.ComponentModel.DataAnnotations;

namespace MCPHub.Common.Configuration;

public class ObservabilityOptions {
    public const string SectionName = "Observability";

    [Required(ErrorMessage = "Service name is required")]
    public string ServiceName { get; set; } = "MCPHub";
    [Required(ErrorMessage = "Service version is required")]
    public string ServiceVersion { get; set; } = "1.0.0";
    [Required(ErrorMessage = "Environment is required")]
    public string Environment { get; set; } = "Development";
}