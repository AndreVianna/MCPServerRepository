using MudBlazor;

namespace MCPHub.WebApp.Services;

/// <summary>
/// Theme service implementing MCP Hub design system
/// </summary>
public static class ThemeService {
    /// <summary>
    /// Gets the MCP Hub theme following UX design specifications
    /// </summary>
    public static MudTheme GetMcpHubTheme() => new() {
        PaletteLight = new PaletteLight {
            // Primary colors from UX Design System
            Primary = "#2563eb",           // MCP Hub Brand Blue
            PrimaryContrastText = "#ffffff",
            PrimaryLighten = "#93c5fd",    // Light Blue for accents
            PrimaryDarken = "#1e3a8a",     // Dark Blue for depth

            // Secondary colors for professional aesthetics
            Secondary = "#6b7280",         // Neutral Gray
            SecondaryContrastText = "#ffffff",
            SecondaryLighten = "#9ca3af",
            SecondaryDarken = "#374151",

            // Security status colors
            Success = "#10b981",           // Security Grade A (Emerald Green)
            SuccessContrastText = "#ffffff",
            SuccessLighten = "#34d399",
            SuccessDarken = "#059669",

            Warning = "#f59e0b",           // Security Grade B (Amber)
            WarningContrastText = "#ffffff",
            WarningLighten = "#fbbf24",
            WarningDarken = "#d97706",

            Error = "#ef4444",             // Security Grade C (Red)
            ErrorContrastText = "#ffffff",
            ErrorLighten = "#f87171",
            ErrorDarken = "#dc2626",

            Info = "#2563eb",              // Primary Blue for info
            InfoContrastText = "#ffffff",
            InfoLighten = "#60a5fa",
            InfoDarken = "#1d4ed8",

            // Background and surface colors
            Background = "#ffffff",
            BackgroundGray = "#f9fafb",    // Gray 50 from design system
            Surface = "#ffffff",

            // Text colors
            TextPrimary = "#111827",       // Gray 900 for primary text
            TextSecondary = "#6b7280",     // Gray 500 for secondary text
            TextDisabled = "#9ca3af",      // Gray 400 for disabled text

            // Action colors
            ActionDefault = "#6b7280",     // Gray 500
            ActionDisabled = "#e5e7eb",    // Gray 200
            ActionDisabledBackground = "#f3f4f6", // Gray 100

            // Divider colors
            Divider = "#e5e7eb",           // Gray 200
            DividerLight = "#f3f4f6",      // Gray 100

            // Table colors
            TableLines = "#e5e7eb",        // Gray 200
            TableStriped = "#f9fafb",      // Gray 50
            TableHover = "#f3f4f6",        // Gray 100

            // Drawer colors
            DrawerBackground = "#ffffff",
            DrawerText = "#111827",
            DrawerIcon = "#6b7280",

            // AppBar colors
            AppbarBackground = "#ffffff",
            AppbarText = "#111827",

            // Hover effects
            HoverOpacity = 0.06,
            GrayDefault = "#6b7280",
            GrayLight = "#d1d5db",
            GrayDark = "#374151",
            GrayLighter = "#f3f4f6",
            GrayDarker = "#1f2937",

            // Overlay
            OverlayDark = "rgba(33,33,33,0.4)",
            OverlayLight = "rgba(255,255,255,0.4)"
        },

        PaletteDark = new PaletteDark {
            // Dark theme colors (for future dark mode support)
            Primary = "#3b82f6",           // Slightly lighter blue for dark mode
            PrimaryContrastText = "#ffffff",

            Secondary = "#9ca3af",
            SecondaryContrastText = "#000000",

            Success = "#10b981",
            Warning = "#f59e0b",
            Error = "#ef4444",
            Info = "#3b82f6",

            Background = "#111827",        // Dark background
            BackgroundGray = "#1f2937",
            Surface = "#1f2937",

            TextPrimary = "#f9fafb",
            TextSecondary = "#d1d5db",
            TextDisabled = "#6b7280",

            ActionDefault = "#9ca3af",
            ActionDisabled = "#374151",
            ActionDisabledBackground = "#1f2937",

            Divider = "#374151",
            DividerLight = "#4b5563",

            TableLines = "#374151",
            TableStriped = "#1f2937",
            TableHover = "#374151",

            DrawerBackground = "#1f2937",
            DrawerText = "#f9fafb",
            DrawerIcon = "#d1d5db",

            AppbarBackground = "#1f2937",
            AppbarText = "#f9fafb"
        },

        Typography = new Typography {
            Default = new Default {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            H1 = new H1 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "6rem",
                FontWeight = 300,
                LineHeight = 1.167,
                LetterSpacing = "-.01562em"
            },
            H2 = new H2 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3.75rem",
                FontWeight = 300,
                LineHeight = 1.2,
                LetterSpacing = "-.00833em"
            },
            H3 = new H3 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "3rem",
                FontWeight = 400,
                LineHeight = 1.167,
                LetterSpacing = "0"
            },
            H4 = new H4 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "2.125rem",
                FontWeight = 400,
                LineHeight = 1.235,
                LetterSpacing = ".00735em"
            },
            H5 = new H5 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = 400,
                LineHeight = 1.334,
                LetterSpacing = "0"
            },
            H6 = new H6 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1.25rem",
                FontWeight = 500,
                LineHeight = 1.6,
                LetterSpacing = ".0075em"
            },
            Button = new Button {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.75,
                LetterSpacing = ".02857em",
                TextTransform = "uppercase"
            },
            Body1 = new Body1 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.5,
                LetterSpacing = ".00938em"
            },
            Body2 = new Body2 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            Caption = new Caption {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".75rem",
                FontWeight = 400,
                LineHeight = 1.66,
                LetterSpacing = ".03333em"
            },
            Subtitle1 = new Subtitle1 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "1rem",
                FontWeight = 400,
                LineHeight = 1.75,
                LetterSpacing = ".00938em"
            },
            Subtitle2 = new Subtitle2 {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = ".875rem",
                FontWeight = 500,
                LineHeight = 1.57,
                LetterSpacing = ".00714em"
            }
        },

        Shadows = new Shadow(),
        LayoutProperties = new LayoutProperties {
            DefaultBorderRadius = "4px",
            AppbarHeight = "64px",
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px"
        },

        ZIndex = new ZIndex()
    };

    /// <summary>
    /// Gets trust tier color based on tier level
    /// </summary>
    /// <param name="trustTier">Trust tier</param>
    /// <returns>Color hex code</returns>
    public static string GetTrustTierColor(string trustTier) => trustTier?.ToLowerInvariant() switch {
        "unverified" => "#6b7280",    // Gray
        "verified" => "#2563eb",      // Primary Blue  
        "trusted" => "#10b981",       // Green
        "enterprise" => "#7c3aed",    // Purple
        _ => "#6b7280"                // Default to gray
    };

    /// <summary>
    /// Gets security grade color based on grade
    /// </summary>
    /// <param name="securityGrade">Security grade</param>
    /// <returns>Color hex code</returns>
    public static string GetSecurityGradeColor(string securityGrade) => securityGrade?.ToUpperInvariant() switch {
        "A" => "#10b981",             // Emerald Green - Excellent
        "B" => "#f59e0b",             // Amber - Good
        "C" => "#ef4444",             // Red - Needs Attention
        "F" => "#7f1d1d",             // Dark Red - Critical
        _ => "#6b7280"                // Default to gray
    };

    /// <summary>
    /// Gets MudBlazor color variant for trust tier
    /// </summary>
    /// <param name="trustTier">Trust tier</param>
    /// <returns>MudBlazor color</returns>
    public static Color GetTrustTierMudColor(string trustTier) => trustTier?.ToLowerInvariant() switch {
        "unverified" => Color.Default,
        "verified" => Color.Primary,
        "trusted" => Color.Success,
        "enterprise" => Color.Secondary,
        _ => Color.Default
    };

    /// <summary>
    /// Gets MudBlazor color variant for security grade
    /// </summary>
    /// <param name="securityGrade">Security grade</param>
    /// <returns>MudBlazor color</returns>
    public static Color GetSecurityGradeMudColor(string securityGrade) => securityGrade?.ToUpperInvariant() switch {
        "A" => Color.Success,
        "B" => Color.Warning,
        "C" => Color.Error,
        "F" => Color.Error,
        _ => Color.Default
    };
}