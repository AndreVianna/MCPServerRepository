using System.Text.Json;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;

using Spectre.Console;

namespace MCPHub.CommandLineApp.Utilities;

/// <summary>
/// Console implementation of output formatter using Spectre.Console
/// </summary>
public class ConsoleOutputFormatter : IOutputFormatter {
    private readonly McpmConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConsoleOutputFormatter(McpmConfiguration configuration) {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _jsonOptions = new JsonSerializerOptions {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Configure Spectre.Console based on configuration
        if (!_configuration.Ui.ColorOutput) {
            AnsiConsole.Profile.Capabilities.ColorSystem = ColorSystem.NoColors;
        }
    }

    /// <inheritdoc />
    public void WriteSuccess(string message) => AnsiConsole.MarkupLine($"[green]✓[/] {message}");

    /// <inheritdoc />
    public void WriteError(string message) => AnsiConsole.MarkupLine($"[red]✗[/] {message}");

    /// <inheritdoc />
    public void WriteWarning(string message) => AnsiConsole.MarkupLine($"[yellow]⚠[/] {message}");

    /// <inheritdoc />
    public void WriteInfo(string message) => AnsiConsole.MarkupLine($"[blue]ℹ[/] {message}");

    /// <inheritdoc />
    public void WriteDebug(string message) {
        if (_configuration.Ui.VerboseErrors) {
            AnsiConsole.MarkupLine($"[gray]DEBUG: {message}[/]");
        }
    }

    /// <inheritdoc />
    public void WriteSearchResults(SearchResultResponse results, string format) {
        switch (format.ToLowerInvariant()) {
            case "json":
                WriteJson(results);
                break;
            case "detailed":
                WriteDetailedSearchResults(results);
                break;
            default:
                WriteTableSearchResults(results);
                break;
        }
    }

    /// <inheritdoc />
    public void WritePackageInfo(PackageInfoResponse packageInfo, string format) {
        switch (format.ToLowerInvariant()) {
            case "json":
                WriteJson(packageInfo);
                break;
            default:
                WriteDetailedPackageInfo(packageInfo);
                break;
        }
    }

    /// <inheritdoc />
    public void WritePackageList(PackageListResponse packages, string format) {
        switch (format.ToLowerInvariant()) {
            case "json":
                WriteJson(packages);
                break;
            case "detailed":
                WriteDetailedPackageList(packages);
                break;
            default:
                WriteTablePackageList(packages);
                break;
        }
    }

    /// <inheritdoc />
    public void WritePackageVersions(PackageVersionsResponse versions, string format) {
        switch (format.ToLowerInvariant()) {
            case "json":
                WriteJson(versions);
                break;
            default:
                WriteTablePackageVersions(versions);
                break;
        }
    }

    /// <inheritdoc />
    public void WriteSecuritySummary(SecuritySummaryResponse security, string format) {
        switch (format.ToLowerInvariant()) {
            case "json":
                WriteJson(security);
                break;
            default:
                WriteDetailedSecuritySummary(security);
                break;
        }
    }

    /// <inheritdoc />
    public void WriteTrustTier(TrustTierResponse trustTier, string format) {
        switch (format.ToLowerInvariant()) {
            case "json":
                WriteJson(trustTier);
                break;
            default:
                WriteDetailedTrustTier(trustTier);
                break;
        }
    }

    /// <inheritdoc />
    public void WriteJson(object data) {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        AnsiConsole.WriteLine(json);
    }

    /// <inheritdoc />
    public void WriteLine() => AnsiConsole.WriteLine();

    /// <inheritdoc />
    public void WriteHeader(string header) => AnsiConsole.MarkupLine($"[bold underline]{header}[/]");

    /// <inheritdoc />
    public void WriteSubHeader(string subHeader) => AnsiConsole.MarkupLine($"[bold]{subHeader}[/]");

    /// <inheritdoc />
    public string GetTrustTierColor(string trustTier) => trustTier.ToLowerInvariant() switch {
        "enterprise" => "green",
        "professional" => "blue",
        "community" => "yellow",
        "unverified" => "red",
        _ => "gray"
    };

    /// <inheritdoc />
    public string GetSecurityGradeColor(string grade) => grade.ToUpperInvariant() switch {
        "A+" or "A" => "green",
        "A-" or "B+" or "B" => "yellow",
        "B-" or "C+" or "C" => "orange",
        _ => "red"
    };

    /// <inheritdoc />
    public string FormatFileSize(long bytes) {
        var units = new[] { "B", "KB", "MB", "GB" };
        var size = (double)bytes;
        var unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1) {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:F1} {units[unitIndex]}";
    }

    /// <inheritdoc />
    public string FormatDownloadCount(long count) {
        if (count >= 1_000_000)
            return $"{count / 1_000_000.0:F1}M";
        if (count >= 1_000)
            return $"{count / 1_000.0:F1}k";
        return count.ToString();
    }

    /// <inheritdoc />
    public string FormatDate(DateTimeOffset date) => date.ToString("yyyy-MM-dd");

    /// <inheritdoc />
    public string FormatRelativeDate(DateTimeOffset date) {
        var timeSpan = DateTimeOffset.UtcNow - date;

        if (timeSpan.TotalDays >= 365)
            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) != 1 ? "s" : "")} ago";
        if (timeSpan.TotalDays >= 30)
            return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) != 1 ? "s" : "")} ago";
        if (timeSpan.TotalDays >= 7)
            return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) != 1 ? "s" : "")} ago";
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";
        if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago";
        if (timeSpan.TotalMinutes >= 1)
            return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago";

        return "just now";
    }

    /// <inheritdoc />
    public string TruncateText(string text, int maxWidth) {
        if (string.IsNullOrEmpty(text) || text.Length <= maxWidth)
            return text;

        return text[..(maxWidth - 3)] + "...";
    }

    private void WriteTableSearchResults(SearchResultResponse results) {
        var table = new Table();
        table.AddColumn("Package");
        table.AddColumn("Version");
        table.AddColumn("Trust Tier");
        table.AddColumn("Security");
        table.AddColumn("Downloads");
        table.AddColumn("Updated");

        foreach (var package in results.Packages) {
            var trustTierColor = GetTrustTierColor(package.TrustTier);
            var securityColor = GetSecurityGradeColor(package.SecurityGrade ?? "N/A");

            table.AddRow(
                TruncateText(package.Name, 30),
                package.LatestVersion,
                $"[{trustTierColor}]{package.TrustTier}[/]",
                $"[{securityColor}]{package.SecurityGrade ?? "N/A"}[/]",
                FormatDownloadCount(package.DownloadCount),
                FormatRelativeDate(package.UpdatedAt)
            );
        }

        AnsiConsole.Write(table);
    }

    private void WriteDetailedSearchResults(SearchResultResponse results) {
        foreach (var package in results.Packages.Take(10)) // Limit for detailed view
        {
            var trustTierColor = GetTrustTierColor(package.TrustTier);
            var securityColor = GetSecurityGradeColor(package.SecurityGrade ?? "N/A");

            AnsiConsole.MarkupLine($"[bold]{package.Name}[/] v{package.LatestVersion}");
            AnsiConsole.MarkupLine($"  {TruncateText(package.Description, 80)}");
            AnsiConsole.MarkupLine($"  Publisher: {package.PublisherName} | Trust: [{trustTierColor}]{package.TrustTier}[/] | Security: [{securityColor}]{package.SecurityGrade ?? "N/A"}[/]");
            AnsiConsole.MarkupLine($"  Downloads: {FormatDownloadCount(package.DownloadCount)} | Updated: {FormatRelativeDate(package.UpdatedAt)}");

            if (package.Tags.Any()) {
                AnsiConsole.MarkupLine($"  Tags: {string.Join(", ", package.Tags.Take(5))}");
            }

            AnsiConsole.WriteLine();
        }

        if (results.Packages.Count > 10) {
            AnsiConsole.MarkupLine($"[gray]... and {results.Packages.Count - 10} more packages[/]");
        }
    }

    private void WriteDetailedPackageInfo(PackageInfoResponse packageInfo) {
        var trustTierColor = GetTrustTierColor(packageInfo.TrustTier);
        var securityColor = GetSecurityGradeColor(packageInfo.SecurityGrade ?? "N/A");

        AnsiConsole.MarkupLine($"[bold blue]📦 {packageInfo.Name}[/] v{packageInfo.Version}");
        AnsiConsole.WriteLine();

        // Basic information
        var infoPanel = new Panel($"""
            [bold]Description:[/] {packageInfo.Description}
            [bold]Publisher:[/] {packageInfo.Publisher.Name} <{packageInfo.Publisher.Email}>
            [bold]Trust Tier:[/] [{trustTierColor}]{packageInfo.TrustTier}[/]
            [bold]Security Grade:[/] [{securityColor}]{packageInfo.SecurityGrade ?? "N/A"}[/]
            [bold]Downloads:[/] {FormatDownloadCount(packageInfo.DownloadCount)}
            [bold]Created:[/] {FormatDate(packageInfo.CreatedAt)}
            [bold]Updated:[/] {FormatRelativeDate(packageInfo.UpdatedAt)}
            """) {
            Header = new PanelHeader("📋 Package Information"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(infoPanel);
        AnsiConsole.WriteLine();

        // Capabilities
        if (packageInfo.Manifest?.Capabilities != null) {
            var capabilities = packageInfo.Manifest.Capabilities;
            var capabilitiesText = $"""
                [bold]Tools:[/] {capabilities.ToolCount} ({string.Join(", ", capabilities.Tools.Take(3))}{(capabilities.Tools.Count > 3 ? "..." : "")})
                [bold]Resources:[/] {capabilities.ResourceCount} ({string.Join(", ", capabilities.Resources.Take(3))}{(capabilities.Resources.Count > 3 ? "..." : "")})
                [bold]Prompts:[/] {capabilities.PromptCount} ({string.Join(", ", capabilities.Prompts.Take(3))}{(capabilities.Prompts.Count > 3 ? "..." : "")})
                """;

            var capabilitiesPanel = new Panel(capabilitiesText) {
                Header = new PanelHeader("⚡ Capabilities"),
                Border = BoxBorder.Rounded
            };
            AnsiConsole.Write(capabilitiesPanel);
            AnsiConsole.WriteLine();
        }

        // Tags
        if (packageInfo.Tags.Any()) {
            AnsiConsole.MarkupLine($"[bold]🏷️  Tags:[/] {string.Join(", ", packageInfo.Tags)}");
            AnsiConsole.WriteLine();
        }
    }

    private void WriteTablePackageList(PackageListResponse packages) => WriteTableSearchResults(new SearchResultResponse {
        Packages = packages.Packages,
        TotalCount = packages.TotalCount,
        Page = packages.Page,
        PageSize = packages.PageSize,
        TotalPages = packages.TotalPages
    });

    private void WriteDetailedPackageList(PackageListResponse packages) => WriteDetailedSearchResults(new SearchResultResponse {
        Packages = packages.Packages,
        TotalCount = packages.TotalCount,
        Page = packages.Page,
        PageSize = packages.PageSize,
        TotalPages = packages.TotalPages
    });

    private void WriteTablePackageVersions(PackageVersionsResponse versions) {
        var table = new Table();
        table.AddColumn("Version");
        table.AddColumn("Status");
        table.AddColumn("Downloads");
        table.AddColumn("Published");

        foreach (var version in versions.Versions) {
            var statusColor = version.Status.ToLowerInvariant() switch {
                "active" => "green",
                "deprecated" => "yellow",
                "yanked" => "red",
                _ => "gray"
            };

            table.AddRow(
                version.Version + (version.IsPrerelease ? " [gray](pre)[/]" : ""),
                $"[{statusColor}]{version.Status}[/]",
                FormatDownloadCount(version.DownloadCount),
                FormatRelativeDate(version.PublishedAt)
            );
        }

        AnsiConsole.Write(table);
    }

    private void WriteDetailedSecuritySummary(SecuritySummaryResponse security) {
        var gradeColor = GetSecurityGradeColor(security.Grade);

        var panel = new Panel($"""
            [bold]Overall Grade:[/] [{gradeColor}]{security.Grade}[/] ({security.Score:F1}/10)
            [bold]Vulnerabilities:[/] {security.Vulnerabilities.Critical} Critical, {security.Vulnerabilities.High} High, {security.Vulnerabilities.Medium} Medium, {security.Vulnerabilities.Low} Low
            [bold]Last Scanned:[/] {(security.LastScanned?.ToString() ?? "Never")}
            [bold]Security Policy:[/] {(security.HasSecurityPolicy ? "✓ Available" : "✗ Not available")}
            """) {
            Header = new PanelHeader("🔒 Security Summary"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);
    }

    private void WriteDetailedTrustTier(TrustTierResponse trustTier) {
        var currentColor = GetTrustTierColor(trustTier.CurrentTier);
        var recommendedColor = GetTrustTierColor(trustTier.RecommendedTier);

        var panel = new Panel($"""
            [bold]Current Tier:[/] [{currentColor}]{trustTier.CurrentTier}[/]
            [bold]Recommended Tier:[/] [{recommendedColor}]{trustTier.RecommendedTier}[/]
            [bold]Trust Score:[/] {trustTier.TrustScore:F1}/10
            [bold]Last Assessed:[/] {FormatRelativeDate(trustTier.LastAssessed)}
            """) {
            Header = new PanelHeader("🛡️ Trust Assessment"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);
    }

    /// <inheritdoc />
    public void WritePublishResult(PublishPackageResponse result, string format) {
        switch (format.ToLowerInvariant()) {
            case "json":
                WriteJson(result);
                break;
            case "detailed":
                WritePublishResultDetailed(result);
                break;
            default:
                WritePublishResultTable(result);
                break;
        }
    }

    /// <inheritdoc />
    public void WriteValidationResult(ValidateManifestResponse result, string format) {
        switch (format.ToLowerInvariant()) {
            case "json":
                WriteJson(result);
                break;
            case "detailed":
                WriteValidationResultDetailed(result);
                break;
            default:
                WriteValidationResultTable(result);
                break;
        }
    }

    private void WritePublishResultTable(PublishPackageResponse result) {
        var table = new Table()
            .Title(result.Success ? "[green]📦 Publishing Result[/]" : "[red]❌ Publishing Failed[/]")
            .Border(TableBorder.Rounded)
            .AddColumn("Property")
            .AddColumn("Value");

        table.AddRow("Status", result.Success ? "[green]Success[/]" : "[red]Failed[/]");

        if (result.Package != null) {
            table.AddRow("Package ID", result.Package.Id.ToString());
            table.AddRow("Package Name", result.Package.Name);
            table.AddRow("Version", result.Package.Version);
            table.AddRow("Status", result.Package.Status);
        }

        table.AddRow("Published At", FormatDate(result.PublishedAt));
        table.AddRow("Processing Time", $"{result.PublishTimeMs}ms");

        if (!string.IsNullOrEmpty(result.StorageUrl)) {
            table.AddRow("Storage URL", result.StorageUrl);
        }

        if (result.Errors.Count > 0) {
            table.AddRow("Errors", $"[red]{result.Errors.Count} error(s)[/]");
        }

        if (result.Warnings.Count > 0) {
            table.AddRow("Warnings", $"[yellow]{result.Warnings.Count} warning(s)[/]");
        }

        AnsiConsole.Write(table);

        if (result.Errors.Count > 0) {
            WriteLine();
            WriteError("Errors:");
            foreach (var error in result.Errors) {
                WriteError($"  - {error}");
            }
        }

        if (result.Warnings.Count > 0) {
            WriteLine();
            WriteWarning("Warnings:");
            foreach (var warning in result.Warnings) {
                WriteWarning($"  - {warning}");
            }
        }
    }

    private void WritePublishResultDetailed(PublishPackageResponse result) {
        var successIcon = result.Success ? "✅" : "❌";
        var title = $"{successIcon} Publishing Result";

        var panel = new Panel($"""
            [bold]Status:[/] {(result.Success ? "[green]Success[/]" : "[red]Failed[/]")}
            [bold]Published At:[/] {FormatDate(result.PublishedAt)}
            [bold]Processing Time:[/] {result.PublishTimeMs}ms
            {(result.Package != null ? $"[bold]Package:[/] {result.Package.Name} v{result.Package.Version}" : "")}
            {(result.Package != null ? $"[bold]Package ID:[/] {result.Package.Id}" : "")}
            {(!string.IsNullOrEmpty(result.StorageUrl) ? $"[bold]Storage URL:[/] {result.StorageUrl}" : "")}
            """) {
            Header = new PanelHeader(title),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);

        if (result.Errors.Count > 0) {
            WriteLine();
            WriteError("Errors:");
            foreach (var error in result.Errors) {
                WriteError($"  - {error}");
            }
        }

        if (result.Warnings.Count > 0) {
            WriteLine();
            WriteWarning("Warnings:");
            foreach (var warning in result.Warnings) {
                WriteWarning($"  - {warning}");
            }
        }
    }

    private void WriteValidationResultTable(ValidateManifestResponse result) {
        var table = new Table()
            .Title(result.IsValid ? "[green]✅ Manifest Validation[/]" : "[red]❌ Manifest Validation Failed[/]")
            .Border(TableBorder.Rounded)
            .AddColumn("Property")
            .AddColumn("Value");

        table.AddRow("Status", result.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]");
        table.AddRow("Errors", result.Errors.Count.ToString());
        table.AddRow("Warnings", result.Warnings.Count.ToString());

        if (result.Manifest != null) {
            table.AddRow("Package Name", result.Manifest.Name ?? "N/A");
            table.AddRow("Version", result.Manifest.Version ?? "N/A");
            table.AddRow("Description", TruncateText(result.Manifest.Description ?? "N/A", 50));
        }

        AnsiConsole.Write(table);

        if (result.Errors.Count > 0) {
            WriteLine();
            WriteError("Validation Errors:");
            foreach (var error in result.Errors) {
                WriteError($"  - {error}");
            }
        }

        if (result.Warnings.Count > 0) {
            WriteLine();
            WriteWarning("Validation Warnings:");
            foreach (var warning in result.Warnings) {
                WriteWarning($"  - {warning}");
            }
        }
    }

    private void WriteValidationResultDetailed(ValidateManifestResponse result) {
        var icon = result.IsValid ? "✅" : "❌";
        var title = $"{icon} Manifest Validation";

        var manifestInfo = result.Manifest != null
            ? $"""            
            [bold]Package:[/] {result.Manifest.Name ?? "N/A"} v{result.Manifest.Version ?? "N/A"}
            [bold]Description:[/] {result.Manifest.Description ?? "N/A"}
            [bold]License:[/] {result.Manifest.License ?? "N/A"}
            """
            : "";

        var panel = new Panel($"""
            [bold]Status:[/] {(result.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]")}
            [bold]Errors:[/] {result.Errors.Count}
            [bold]Warnings:[/] {result.Warnings.Count}
            {manifestInfo}
            """) {
            Header = new PanelHeader(title),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);

        if (result.Errors.Count > 0) {
            WriteLine();
            WriteError("Validation Errors:");
            foreach (var error in result.Errors) {
                WriteError($"  - {error}");
            }
        }

        if (result.Warnings.Count > 0) {
            WriteLine();
            WriteWarning("Validation Warnings:");
            foreach (var warning in result.Warnings) {
                WriteWarning($"  - {warning}");
            }
        }
    }
}