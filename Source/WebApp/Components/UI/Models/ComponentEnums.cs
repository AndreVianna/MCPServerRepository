namespace MCPHub.WebApp.Components.UI.Models;

/// <summary>
/// Types of search suggestions
/// </summary>
public enum SuggestionType {
    Query,
    Package,
    Category,
    Publisher,
    AIRecommendation,
    Tag,
    RecentSearch,
}

/// <summary>
/// Chart types for analytics components
/// </summary>
public enum ChartType {
    Line,
    Bar,
    Donut,
    Area,
    Pie,
}

/// <summary>
/// Sidebar position for responsive layouts
/// </summary>
public enum SidebarPosition {
    Left,
    Right,
}

/// <summary>
/// Container maximum width options
/// </summary>
public enum ContainerMaxWidth {
    Small,      // 640px
    Medium,     // 768px
    Large,      // 1024px
    ExtraLarge, // 1280px
    Full, // 100%
}

/// <summary>
/// Security badge styles
/// </summary>
public enum SecurityBadgeStyle {
    Compact,
    Standard,
    Detailed,
}

/// <summary>
/// Trust tier indicator styles
/// </summary>
public enum TrustTierStyle {
    Badge,
    Card,
    Inline,
}

/// <summary>
/// Package card display modes
/// </summary>
public enum PackageCardMode {
    Compact,
    Standard,
    Detailed,
    Grid,
    List,
}

/// <summary>
/// Search bar sizes
/// </summary>
public enum SearchBarSize {
    Small,
    Medium,
    Large,
}

/// <summary>
/// Analytics time ranges
/// </summary>
public enum AnalyticsTimeRange {
    Last7Days,
    Last30Days,
    Last90Days,
    LastYear,
    AllTime,
}

/// <summary>
/// Component theme variants
/// </summary>
public enum ComponentTheme {
    Default,
    Primary,
    Secondary,
    Success,
    Warning,
    Error,
    Info,
}

/// <summary>
/// Loading states for components
/// </summary>
public enum LoadingState {
    NotStarted,
    Loading,
    Loaded,
    Error,
    Empty,
}

/// <summary>
/// Accessibility priorities for screen readers
/// </summary>
public enum AccessibilityPriority {
    Low,
    Medium,
    High,
    Critical,
}