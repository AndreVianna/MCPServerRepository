# MCP Hub Enhanced Component Library Specification

**Version**: Phase 2B Implementation Guide  
**Date**: July 28, 2025  
**Status**: Complete Component Specifications for MudBlazor  

---

## Overview

This document provides detailed specifications for implementing MCP Hub's component library using MudBlazor. Each component includes technical implementation details, accessibility requirements, and responsive design considerations tailored for the Phase 2B web application development.

---

## Core Security Components

### 1. SecurityGradeBadge Component

#### Purpose
Displays package security grades with visual hierarchy and detailed breakdown on interaction.

#### MudBlazor Implementation
```razor
@using MudBlazor

<MudTooltip Text="@TooltipText" Placement="Placement.Top" Arrow="true">
    <MudChip Size="Size.Medium" 
             Color="@GetGradeColor()" 
             Class="@GetGradeClass()"
             Style="@GetGradeStyle()"
             @onclick="ShowDetailedReport">
        <MudIcon Icon="@GetGradeIcon()" Size="Size.Small" Class="mr-1" />
        @Grade
        @if (ShowScore)
        {
            <span class="ml-1 text-xs">(@Score.ToString("F1"))</span>
        }
    </MudChip>
</MudTooltip>

@code {
    [Parameter] public string Grade { get; set; } = "A+";
    [Parameter] public double Score { get; set; } = 9.4;
    [Parameter] public bool ShowScore { get; set; } = true;
    [Parameter] public SecurityScanResult? DetailedReport { get; set; }
    [Parameter] public EventCallback OnDetailClick { get; set; }

    private string TooltipText => $"Security Grade: {Grade} ({Score:F1}/10.0)";

    private Color GetGradeColor() => Grade switch
    {
        "A+" or "A" => Color.Success,
        "B+" or "B" => Color.Warning,
        "C+" or "C" => Color.Error,
        "F" => Color.Error,
        _ => Color.Default
    };

    private string GetGradeClass() => Grade switch
    {
        "A+" => "security-grade-aplus",
        "A" => "security-grade-a",
        "B+" or "B" => "security-grade-b",
        "C+" or "C" => "security-grade-c",
        "F" => "security-grade-f",
        _ => "security-grade-unknown"
    };

    private string GetGradeStyle() => Grade switch
    {
        "A+" => "background: linear-gradient(135deg, #10b981, #059669); color: white;",
        "A" => "background: linear-gradient(135deg, #34d399, #10b981); color: white;",
        "B+" or "B" => "background: linear-gradient(135deg, #f59e0b, #d97706); color: white;",
        "C+" or "C" => "background: linear-gradient(135deg, #ef4444, #dc2626); color: white;",
        "F" => "background: linear-gradient(135deg, #7f1d1d, #991b1b); color: white;",
        _ => "background: #6b7280; color: white;"
    };

    private string GetGradeIcon() => Grade switch
    {
        "A+" => Icons.Material.Filled.Security,
        "A" => Icons.Material.Filled.Verified,
        "B+" or "B" => Icons.Material.Filled.Warning,
        "C+" or "C" => Icons.Material.Filled.Error,
        "F" => Icons.Material.Filled.Dangerous,
        _ => Icons.Material.Filled.Help
    };

    private async Task ShowDetailedReport()
    {
        await OnDetailClick.InvokeAsync();
    }
}
```

#### CSS Styles
```css
.security-grade-aplus {
    box-shadow: 0 2px 8px rgba(16, 185, 129, 0.3);
    border: 1px solid rgba(16, 185, 129, 0.2);
}

.security-grade-a {
    box-shadow: 0 2px 8px rgba(52, 211, 153, 0.3);
    border: 1px solid rgba(52, 211, 153, 0.2);
}

.security-grade-b {
    box-shadow: 0 2px 8px rgba(245, 158, 11, 0.3);
    border: 1px solid rgba(245, 158, 11, 0.2);
}

.security-grade-c {
    box-shadow: 0 2px 8px rgba(239, 68, 68, 0.3);
    border: 1px solid rgba(239, 68, 68, 0.2);
}

.security-grade-f {
    box-shadow: 0 2px 8px rgba(127, 29, 29, 0.3);
    border: 1px solid rgba(127, 29, 29, 0.2);
    animation: pulse-danger 2s infinite;
}

@keyframes pulse-danger {
    0%, 100% { opacity: 1; }
    50% { opacity: 0.8; }
}
```

#### Accessibility Features
- **ARIA Label**: Comprehensive description including grade and score
- **Keyboard Navigation**: Focusable and activatable with Enter/Space
- **Screen Reader**: Announced as "Security Grade A+ 9.4 out of 10, button"
- **Color Independence**: Uses icons and text in addition to color

---

### 2. TrustTierIndicator Component

#### Purpose
Shows package trust tier with progression indicators and publisher verification status.

#### MudBlazor Implementation
```razor
@using MudBlazor

<MudTooltip Text="@GetTooltipText()" Placement="Placement.Top" Arrow="true">
    <MudChip Size="Size.Small" 
             Color="@GetTierColor()" 
             Variant="@GetTierVariant()"
             Class="@GetTierClass()"
             @onclick="ShowTierDetails">
        <MudIcon Icon="@GetTierIcon()" Size="Size.Small" Class="mr-1" />
        @GetTierDisplayName()
        @if (ShowProgressIndicator && CanProgress())
        {
            <MudIcon Icon="Icons.Material.Filled.TrendingUp" 
                     Size="Size.Small" 
                     Class="ml-1 tier-progress-icon" />
        }
    </MudChip>
</MudTooltip>

@if (ShowProgressBar)
{
    <MudProgressLinear Value="@GetProgressValue()" 
                       Color="@GetTierColor()" 
                       Size="Size.Small"
                       Class="mt-1 tier-progress-bar" />
}

@code {
    [Parameter] public TrustTier Tier { get; set; } = TrustTier.Unverified;
    [Parameter] public bool ShowProgressIndicator { get; set; } = false;
    [Parameter] public bool ShowProgressBar { get; set; } = false;
    [Parameter] public TrustTierProgress? Progress { get; set; }
    [Parameter] public EventCallback OnDetailsClick { get; set; }

    private string GetTooltipText() => Tier switch
    {
        TrustTier.Unverified => "Unverified publisher - Package available but publisher identity not confirmed",
        TrustTier.Verified => "Verified publisher - Identity confirmed, basic package validation",
        TrustTier.Trusted => "Trusted publisher - Proven track record, community endorsed",
        TrustTier.Enterprise => "Enterprise publisher - Full security audit, SLA available",
        _ => "Unknown trust tier"
    };

    private Color GetTierColor() => Tier switch
    {
        TrustTier.Unverified => Color.Default,
        TrustTier.Verified => Color.Primary,
        TrustTier.Trusted => Color.Success,
        TrustTier.Enterprise => Color.Secondary,
        _ => Color.Default
    };

    private Variant GetTierVariant() => Tier switch
    {
        TrustTier.Unverified => Variant.Outlined,
        _ => Variant.Filled
    };

    private string GetTierClass() => $"trust-tier trust-tier-{Tier.ToString().ToLower()}";

    private string GetTierIcon() => Tier switch
    {
        TrustTier.Unverified => Icons.Material.Outlined.Help,
        TrustTier.Verified => Icons.Material.Filled.Verified,
        TrustTier.Trusted => Icons.Material.Filled.Shield,
        TrustTier.Enterprise => Icons.Material.Filled.BusinessCenter,
        _ => Icons.Material.Outlined.Help
    };

    private string GetTierDisplayName() => Tier switch
    {
        TrustTier.Unverified => "Unverified",
        TrustTier.Verified => "Verified",
        TrustTier.Trusted => "Trusted",
        TrustTier.Enterprise => "Enterprise",
        _ => "Unknown"
    };

    private bool CanProgress() => Tier != TrustTier.Enterprise && Progress?.CanAdvance == true;

    private double GetProgressValue() => Progress?.CompletionPercentage ?? 0;

    private async Task ShowTierDetails()
    {
        await OnDetailsClick.InvokeAsync();
    }
}
```

#### CSS Styles
```css
.trust-tier {
    transition: all 0.2s ease-in-out;
}

.trust-tier:hover {
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.trust-tier-unverified {
    border-color: #9ca3af;
    color: #6b7280;
}

.trust-tier-verified {
    background: linear-gradient(135deg, #2563eb, #1d4ed8);
}

.trust-tier-trusted {
    background: linear-gradient(135deg, #10b981, #059669);
}

.trust-tier-enterprise {
    background: linear-gradient(135deg, #7c3aed, #6d28d9);
}

.tier-progress-icon {
    animation: bounce-subtle 2s infinite;
}

.tier-progress-bar {
    border-radius: 2px;
    height: 3px;
}

@keyframes bounce-subtle {
    0%, 100% { transform: translateY(0); }
    50% { transform: translateY(-2px); }
}
```

---

## Package Display Components

### 3. PackageCard Component

#### Purpose
Comprehensive package display with security information, metadata, and quick actions.

#### MudBlazor Implementation
```razor
@using MudBlazor

<MudCard Class="@GetCardClass()" Style="@GetCardStyle()">
    @if (IsFeatured)
    {
        <MudCardHeader Class="featured-header">
            <CardHeaderContent>
                <MudIcon Icon="Icons.Material.Filled.Star" Color="Color.Warning" />
                <MudText Typo="Typo.caption" Class="ml-1">Featured Package</MudText>
            </CardHeaderContent>
        </MudCardHeader>
    }

    <MudCardContent Class="package-card-content">
        <!-- Package Header -->
        <div class="package-header d-flex justify-space-between align-center mb-2">
            <div class="package-name-section">
                <MudLink Href="@GetPackageUrl()" Class="package-name-link">
                    <MudText Typo="Typo.h6" Class="package-name">
                        @Package.Name
                    </MudText>
                </MudLink>
                <MudText Typo="Typo.caption" Class="package-version">
                    v@Package.LatestVersion
                </MudText>
            </div>
            
            <div class="package-indicators d-flex align-center">
                <SecurityGradeBadge Grade="@Package.SecurityGrade" 
                                   Score="@Package.SecurityScore" 
                                   ShowScore="false" />
                <TrustTierIndicator Tier="@Package.TrustTier" 
                                   Class="ml-2" />
                @if (Package.IsTrending)
                {
                    <MudIcon Icon="Icons.Material.Filled.TrendingUp" 
                             Color="Color.Success" 
                             Size="Size.Small" 
                             Class="ml-2" 
                             Title="Trending package" />
                }
            </div>
        </div>

        <!-- Package Description -->
        <MudText Typo="Typo.body2" Class="package-description mb-3">
            @(Package.Description?.Length > MaxDescriptionLength 
                ? Package.Description.Substring(0, MaxDescriptionLength) + "..." 
                : Package.Description)
        </MudText>

        <!-- Package Metadata -->
        <div class="package-metadata d-flex flex-wrap align-center mb-3">
            <div class="metadata-item d-flex align-center mr-4">
                <MudIcon Icon="Icons.Material.Filled.Download" Size="Size.Small" Class="mr-1" />
                <MudText Typo="Typo.caption">@FormatDownloads(Package.WeeklyDownloads)/week</MudText>
            </div>
            
            <div class="metadata-item d-flex align-center mr-4">
                <MudIcon Icon="Icons.Material.Filled.Star" Size="Size.Small" Class="mr-1" />
                <MudText Typo="Typo.caption">@Package.Rating.ToString("F1")</MudText>
            </div>
            
            <div class="metadata-item d-flex align-center mr-4">
                <MudIcon Icon="Icons.Material.Filled.Schedule" Size="Size.Small" Class="mr-1" />
                <MudText Typo="Typo.caption">@FormatLastUpdated(Package.LastUpdated)</MudText>
            </div>
            
            <div class="metadata-item d-flex align-center">
                <MudIcon Icon="Icons.Material.Filled.Person" Size="Size.Small" Class="mr-1" />
                <MudText Typo="Typo.caption">@Package.Publisher.Name</MudText>
            </div>
        </div>

        <!-- Package Tags -->
        @if (Package.Tags?.Any() == true)
        {
            <div class="package-tags d-flex flex-wrap mb-3">
                @foreach (var tag in Package.Tags.Take(MaxVisibleTags))
                {
                    <MudChip Size="Size.Small" 
                             Color="Color.Default" 
                             Variant="Variant.Outlined"
                             Class="mr-1 mb-1">
                        @tag
                    </MudChip>
                }
                @if (Package.Tags.Count > MaxVisibleTags)
                {
                    <MudChip Size="Size.Small" 
                             Color="Color.Default" 
                             Variant="Variant.Text"
                             Class="mr-1 mb-1">
                        +@(Package.Tags.Count - MaxVisibleTags) more
                    </MudChip>
                }
            </div>
        }

        @if (ShowAIInsights && Package.AIRelevanceScore.HasValue)
        {
            <MudAlert Severity="Severity.Info" Dense="true" Class="mb-3">
                <div class="d-flex align-center">
                    <MudIcon Icon="Icons.Material.Filled.Psychology" Size="Size.Small" Class="mr-2" />
                    <MudText Typo="Typo.caption">
                        @(Package.AIRelevanceScore.Value)% relevant to your search
                    </MudText>
                </div>
            </MudAlert>
        }
    </MudCardContent>

    <MudCardActions Class="package-card-actions justify-space-between">
        <div class="action-buttons">
            <MudButton Size="Size.Small" 
                       Color="Color.Primary" 
                       Variant="Variant.Filled"
                       StartIcon="Icons.Material.Filled.Download"
                       @onclick="@(() => OnInstallClick.InvokeAsync(Package))">
                Install
            </MudButton>
            
            <MudButton Size="Size.Small" 
                       Color="Color.Default" 
                       Variant="Variant.Outlined"
                       StartIcon="Icons.Material.Filled.Info"
                       Class="ml-2"
                       @onclick="@(() => OnDetailsClick.InvokeAsync(Package))">
                Details
            </MudButton>
        </div>
        
        <div class="secondary-actions">
            <MudIconButton Icon="@(Package.IsStarred ? Icons.Material.Filled.Star : Icons.Material.Outlined.Star)"
                           Color="@(Package.IsStarred ? Color.Warning : Color.Default)"
                           Size="Size.Small"
                           @onclick="@(() => OnStarClick.InvokeAsync(Package))"
                           Title="@(Package.IsStarred ? "Remove from favorites" : "Add to favorites")" />
            
            <MudIconButton Icon="Icons.Material.Filled.Share"
                           Color="Color.Default"
                           Size="Size.Small"
                           @onclick="@(() => OnShareClick.InvokeAsync(Package))"
                           Title="Share package" />
            
            <MudIconButton Icon="Icons.Material.Filled.MoreVert"
                           Color="Color.Default"
                           Size="Size.Small"
                           @onclick="@(() => OnMoreClick.InvokeAsync(Package))"
                           Title="More options" />
        </div>
    </MudCardActions>
</MudCard>

@code {
    [Parameter] public PackageInfo Package { get; set; } = null!;
    [Parameter] public bool IsFeatured { get; set; } = false;
    [Parameter] public bool ShowAIInsights { get; set; } = false;
    [Parameter] public int MaxDescriptionLength { get; set; } = 150;
    [Parameter] public int MaxVisibleTags { get; set; } = 4;
    [Parameter] public EventCallback<PackageInfo> OnInstallClick { get; set; }
    [Parameter] public EventCallback<PackageInfo> OnDetailsClick { get; set; }
    [Parameter] public EventCallback<PackageInfo> OnStarClick { get; set; }
    [Parameter] public EventCallback<PackageInfo> OnShareClick { get; set; }
    [Parameter] public EventCallback<PackageInfo> OnMoreClick { get; set; }

    private string GetCardClass() => $"package-card {(IsFeatured ? "featured-card" : "")} {(Package.IsTrending ? "trending-card" : "")}";
    
    private string GetCardStyle() => IsFeatured 
        ? "border: 2px solid var(--mud-palette-primary); box-shadow: 0 4px 16px rgba(37, 99, 235, 0.2);"
        : "";

    private string GetPackageUrl() => $"/package/{Package.Publisher.Name}/{Package.Name}";

    private string FormatDownloads(long downloads)
    {
        return downloads switch
        {
            >= 1000000 => $"{downloads / 1000000.0:F1}M",
            >= 1000 => $"{downloads / 1000.0:F1}K",
            _ => downloads.ToString()
        };
    }

    private string FormatLastUpdated(DateTime lastUpdated)
    {
        var timeSpan = DateTime.UtcNow - lastUpdated;
        return timeSpan.TotalDays switch
        {
            < 1 => "Today",
            < 2 => "Yesterday",
            < 7 => $"{(int)timeSpan.TotalDays}d ago",
            < 30 => $"{(int)(timeSpan.TotalDays / 7)}w ago",
            < 365 => $"{(int)(timeSpan.TotalDays / 30)}mo ago",
            _ => $"{(int)(timeSpan.TotalDays / 365)}y ago"
        };
    }
}
```

#### CSS Styles
```css
.package-card {
    transition: all 0.2s ease-in-out;
    border-radius: 8px;
    overflow: hidden;
}

.package-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
}

.featured-card {
    position: relative;
}

.featured-header {
    background: linear-gradient(135deg, #fbbf24, #f59e0b);
    color: white;
    padding: 8px 16px;
}

.trending-card::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 4px;
    height: 100%;
    background: linear-gradient(to bottom, #10b981, #059669);
    z-index: 1;
}

.package-name-link {
    text-decoration: none;
    color: inherit;
}

.package-name-link:hover .package-name {
    color: var(--mud-palette-primary);
}

.package-description {
    line-height: 1.5;
    color: var(--mud-palette-text-secondary);
}

.metadata-item {
    font-size: 0.75rem;
    color: var(--mud-palette-text-secondary);
}

.package-tags .mud-chip {
    font-size: 0.7rem;
    height: 24px;
}

.package-card-actions {
    padding: 12px 16px;
    background-color: var(--mud-palette-background-grey);
}

@media (max-width: 768px) {
    .package-card-actions {
        flex-direction: column;
        gap: 8px;
    }
    
    .action-buttons {
        width: 100%;
        display: flex;
        gap: 8px;
    }
    
    .action-buttons .mud-button {
        flex: 1;
    }
}
```

---

## Search & Filter Components

### 4. EnhancedSearchBar Component

#### Purpose
Advanced search with AI-powered suggestions, voice input, and real-time filtering.

#### MudBlazor Implementation
```razor
@using MudBlazor

<div class="enhanced-search-container">
    <MudAutocomplete T="SearchSuggestion"
                     @bind-Value="@SelectedSuggestion"
                     SearchFunc="@SearchSuggestions"
                     ToStringFunc="@(s => s?.DisplayText ?? "")"
                     Placeholder="@Placeholder"
                     Variant="Variant.Outlined"
                     AdornmentIcon="@Icons.Material.Filled.Search"
                     AdornmentColor="Color.Primary"
                     Class="@GetSearchClass()"
                     @onkeypress="HandleKeyPress"
                     ResetValueOnEmptyText="true"
                     CoerceText="true"
                     MaxItems="8">
        
        <ItemTemplate Context="suggestion">
            <div class="search-suggestion-item d-flex align-center">
                <MudIcon Icon="@GetSuggestionIcon(suggestion)" 
                         Size="Size.Small" 
                         Class="mr-2" />
                <div class="suggestion-content">
                    <MudText Typo="Typo.body2" Class="suggestion-text">
                        @suggestion.DisplayText
                    </MudText>
                    @if (!string.IsNullOrEmpty(suggestion.Description))
                    {
                        <MudText Typo="Typo.caption" Class="suggestion-description">
                            @suggestion.Description
                        </MudText>
                    }
                </div>
                @if (suggestion.Type == SuggestionType.AIRecommendation)
                {
                    <MudChip Size="Size.Small" 
                             Color="Color.Primary" 
                             Variant="Variant.Text"
                             Class="ml-auto">
                        AI
                    </MudChip>
                }
            </div>
        </ItemTemplate>
        
        <ItemSelectedTemplate Context="suggestion">
            <MudText>@suggestion.DisplayText</MudText>
        </ItemSelectedTemplate>

        <MoreItemsTemplate>
            <div class="search-more-items d-flex align-center justify-center pa-2">
                <MudIcon Icon="Icons.Material.Filled.ExpandMore" Class="mr-1" />
                <MudText Typo="Typo.caption">More suggestions...</MudText>
            </div>
        </MoreItemsTemplate>
    </MudAutocomplete>

    <!-- Voice Search Button -->
    @if (EnableVoiceSearch && IsVoiceSupported)
    {
        <MudIconButton Icon="@GetVoiceIcon()"
                       Color="@GetVoiceColor()"
                       Size="Size.Medium"
                       Class="voice-search-button"
                       @onclick="ToggleVoiceSearch"
                       Title="@GetVoiceTooltip()" />
    }

    <!-- Advanced Search Toggle -->
    @if (EnableAdvancedSearch)
    {
        <MudIconButton Icon="Icons.Material.Filled.Tune"
                       Color="Color.Default"
                       Size="Size.Medium"
                       Class="advanced-search-button"
                       @onclick="ToggleAdvancedSearch"
                       Title="Advanced search options" />
    }

    <!-- Search Filters (Expandable) -->
    @if (ShowAdvancedFilters)
    {
        <MudCollapse Expanded="@ShowAdvancedFilters">
            <div class="advanced-filters mt-3 pa-3">
                <MudGrid>
                    <MudItem xs="12" sm="6" md="3">
                        <MudSelect T="string" 
                                   @bind-Value="@SelectedCategory"
                                   Label="Category"
                                   Variant="Variant.Outlined"
                                   Dense="true">
                            <MudSelectItem Value="@("")">All Categories</MudSelectItem>
                            @foreach (var category in Categories)
                            {
                                <MudSelectItem Value="@category.Key">@category.Value</MudSelectItem>
                            }
                        </MudSelect>
                    </MudItem>
                    
                    <MudItem xs="12" sm="6" md="3">
                        <MudSelect T="TrustTier?" 
                                   @bind-Value="@SelectedTrustTier"
                                   Label="Trust Tier"
                                   Variant="Variant.Outlined"
                                   Dense="true">
                            <MudSelectItem Value="@((TrustTier?)null)">Any Trust Level</MudSelectItem>
                            @foreach (TrustTier tier in Enum.GetValues<TrustTier>())
                            {
                                <MudSelectItem Value="@tier">@tier.ToString()</MudSelectItem>
                            }
                        </MudSelect>
                    </MudItem>
                    
                    <MudItem xs="12" sm="6" md="3">
                        <MudSelect T="string" 
                                   @bind-Value="@SelectedSecurityGrade"
                                   Label="Min Security Grade"
                                   Variant="Variant.Outlined"
                                   Dense="true">
                            <MudSelectItem Value="@("")">Any Grade</MudSelectItem>
                            <MudSelectItem Value="A+">A+ and above</MudSelectItem>
                            <MudSelectItem Value="A">A and above</MudSelectItem>
                            <MudSelectItem Value="B">B and above</MudSelectItem>
                            <MudSelectItem Value="C">C and above</MudSelectItem>
                        </MudSelect>
                    </MudItem>
                    
                    <MudItem xs="12" sm="6" md="3">
                        <MudSelect T="string" 
                                   @bind-Value="@SelectedTimeframe"
                                   Label="Updated"
                                   Variant="Variant.Outlined"
                                   Dense="true">
                            <MudSelectItem Value="@("")">Any time</MudSelectItem>
                            <MudSelectItem Value="week">This week</MudSelectItem>
                            <MudSelectItem Value="month">This month</MudSelectItem>
                            <MudSelectItem Value="3months">Last 3 months</MudSelectItem>
                        </MudSelect>
                    </MudItem>
                </MudGrid>
            </div>
        </MudCollapse>
    }
</div>

@code {
    [Parameter] public string Placeholder { get; set; } = "Search packages...";
    [Parameter] public bool EnableVoiceSearch { get; set; } = true;
    [Parameter] public bool EnableAdvancedSearch { get; set; } = true;
    [Parameter] public bool IsLargeSize { get; set; } = false;
    [Parameter] public EventCallback<SearchQuery> OnSearch { get; set; }
    [Parameter] public Dictionary<string, string> Categories { get; set; } = new();

    private SearchSuggestion? SelectedSuggestion;
    private bool ShowAdvancedFilters = false;
    private bool IsVoiceListening = false;
    private bool IsVoiceSupported = false;
    private string SelectedCategory = "";
    private TrustTier? SelectedTrustTier;
    private string SelectedSecurityGrade = "";
    private string SelectedTimeframe = "";

    protected override async Task OnInitializedAsync()
    {
        // Check if browser supports voice recognition
        IsVoiceSupported = await CheckVoiceSupport();
    }

    private string GetSearchClass() => IsLargeSize ? "search-bar-large" : "search-bar-normal";

    private async Task<IEnumerable<SearchSuggestion>> SearchSuggestions(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
            return Array.Empty<SearchSuggestion>();

        // Simulate API call for suggestions
        await Task.Delay(300); // Debounce
        
        return new[]
        {
            new SearchSuggestion { DisplayText = $"{value} packages", Type = SuggestionType.Query, Description = "Search all packages" },
            new SearchSuggestion { DisplayText = $"{value} tools", Type = SuggestionType.Category, Description = "In Tools category" },
            new SearchSuggestion { DisplayText = $"AI-powered {value}", Type = SuggestionType.AIRecommendation, Description = "AI suggestion" }
        };
    }

    private string GetSuggestionIcon(SearchSuggestion suggestion) => suggestion.Type switch
    {
        SuggestionType.Query => Icons.Material.Filled.Search,
        SuggestionType.Package => Icons.Material.Filled.Inventory,
        SuggestionType.Category => Icons.Material.Filled.Category,
        SuggestionType.Publisher => Icons.Material.Filled.Person,
        SuggestionType.AIRecommendation => Icons.Material.Filled.Psychology,
        _ => Icons.Material.Filled.Search
    };

    private string GetVoiceIcon() => IsVoiceListening 
        ? Icons.Material.Filled.MicOff 
        : Icons.Material.Filled.Mic;

    private Color GetVoiceColor() => IsVoiceListening 
        ? Color.Error 
        : Color.Default;

    private string GetVoiceTooltip() => IsVoiceListening 
        ? "Stop voice search" 
        : "Start voice search";

    private async Task ToggleVoiceSearch()
    {
        if (IsVoiceListening)
        {
            await StopVoiceRecognition();
        }
        else
        {
            await StartVoiceRecognition();
        }
    }

    private void ToggleAdvancedSearch()
    {
        ShowAdvancedFilters = !ShowAdvancedFilters;
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && SelectedSuggestion != null)
        {
            await PerformSearch();
        }
    }

    private async Task PerformSearch()
    {
        var searchQuery = new SearchQuery
        {
            Text = SelectedSuggestion?.DisplayText ?? "",
            Category = SelectedCategory,
            TrustTier = SelectedTrustTier,
            SecurityGrade = SelectedSecurityGrade,
            Timeframe = SelectedTimeframe
        };

        await OnSearch.InvokeAsync(searchQuery);
    }

    private async Task<bool> CheckVoiceSupport()
    {
        // Implementation would check browser capabilities
        return true; // Simplified for example
    }

    private async Task StartVoiceRecognition()
    {
        IsVoiceListening = true;
        // Implementation would start browser voice recognition
        StateHasChanged();
    }

    private async Task StopVoiceRecognition()
    {
        IsVoiceListening = false;
        // Implementation would stop browser voice recognition
        StateHasChanged();
    }
}

public class SearchSuggestion
{
    public string DisplayText { get; set; } = "";
    public string Description { get; set; } = "";
    public SuggestionType Type { get; set; }
}

public enum SuggestionType
{
    Query,
    Package,
    Category,
    Publisher,
    AIRecommendation
}

public class SearchQuery
{
    public string Text { get; set; } = "";
    public string Category { get; set; } = "";
    public TrustTier? TrustTier { get; set; }
    public string SecurityGrade { get; set; } = "";
    public string Timeframe { get; set; } = "";
}
```

---

## Analytics & Visualization Components

### 5. AnalyticsChart Component

#### Purpose
Interactive charts for download trends, security metrics, and performance data.

#### MudBlazor Implementation
```razor
@using MudBlazor

<MudCard Class="analytics-chart-card">
    <MudCardHeader>
        <CardHeaderContent>
            <div class="d-flex justify-space-between align-center">
                <div>
                    <MudText Typo="Typo.h6">@Title</MudText>
                    @if (!string.IsNullOrEmpty(Subtitle))
                    {
                        <MudText Typo="Typo.caption" Class="text-secondary">@Subtitle</MudText>
                    }
                </div>
                <div class="chart-controls d-flex align-center">
                    @if (ShowTimeRangeSelector)
                    {
                        <MudSelect T="string" 
                                   @bind-Value="@SelectedTimeRange"
                                   Variant="Variant.Outlined"
                                   Dense="true"
                                   Class="mr-2"
                                   Style="min-width: 120px;">
                            <MudSelectItem Value="7d">Last 7 days</MudSelectItem>
                            <MudSelectItem Value="30d">Last 30 days</MudSelectItem>
                            <MudSelectItem Value="90d">Last 90 days</MudSelectItem>
                            <MudSelectItem Value="1y">Last year</MudSelectItem>
                        </MudSelect>
                    }
                    
                    <MudIconButton Icon="Icons.Material.Filled.Refresh"
                                   Color="Color.Default"
                                   Size="Size.Small"
                                   @onclick="RefreshData"
                                   Title="Refresh data" />
                    
                    <MudIconButton Icon="Icons.Material.Filled.Download"
                                   Color="Color.Default"
                                   Size="Size.Small"
                                   @onclick="ExportData"
                                   Title="Export data" />
                </div>
            </div>
        </CardHeaderContent>
    </MudCardHeader>

    <MudCardContent Class="chart-content">
        @if (IsLoading)
        {
            <div class="chart-loading d-flex justify-center align-center" style="height: @($"{Height}px")">
                <MudProgressCircular Indeterminate="true" Size="Size.Medium" />
            </div>
        }
        else if (HasError)
        {
            <div class="chart-error d-flex flex-column justify-center align-center" style="height: @($"{Height}px")">
                <MudIcon Icon="Icons.Material.Filled.Error" Color="Color.Error" Size="Size.Large" />
                <MudText Typo="Typo.body2" Class="mt-2">Failed to load chart data</MudText>
                <MudButton Color="Color.Primary" 
                           Variant="Variant.Text" 
                           StartIcon="Icons.Material.Filled.Refresh"
                           Class="mt-2"
                           @onclick="RefreshData">
                    Retry
                </MudButton>
            </div>
        }
        else
        {
            <div class="chart-container" style="height: @($"{Height}px")">
                @if (ChartType == ChartType.Line)
                {
                    <MudChart ChartType="MudBlazor.ChartType.Line"
                              ChartSeries="@ChartSeries"
                              @bind-SelectedIndex="SelectedIndex"
                              XAxisLabels="@XAxisLabels"
                              Width="100%"
                              Height="@($"{Height}px")"
                              ChartOptions="@GetChartOptions()" />
                }
                else if (ChartType == ChartType.Bar)
                {
                    <MudChart ChartType="MudBlazor.ChartType.Bar"
                              ChartSeries="@ChartSeries"
                              @bind-SelectedIndex="SelectedIndex"
                              XAxisLabels="@XAxisLabels"
                              Width="100%"
                              Height="@($"{Height}px")"
                              ChartOptions="@GetChartOptions()" />
                }
                else if (ChartType == ChartType.Donut)
                {
                    <MudChart ChartType="MudBlazor.ChartType.Donut"
                              ChartSeries="@ChartSeries"
                              @bind-SelectedIndex="SelectedIndex"
                              Width="100%"
                              Height="@($"{Height}px")"
                              ChartOptions="@GetChartOptions()" />
                }
            </div>

            @if (ShowDataTable)
            {
                <div class="chart-data-table mt-4">
                    <MudTable Items="@GetTableData()" 
                              Dense="true" 
                              Striped="true"
                              Class="chart-table">
                        <HeaderContent>
                            <MudTh>Period</MudTh>
                            @foreach (var series in ChartSeries)
                            {
                                <MudTh>@series.Name</MudTh>
                            }
                        </HeaderContent>
                        <RowTemplate>
                            <MudTd>@context.Period</MudTd>
                            @foreach (var value in context.Values)
                            {
                                <MudTd>@value.ToString("N0")</MudTd>
                            }
                        </RowTemplate>
                    </MudTable>
                </div>
            }
        }
    </MudCardContent>

    @if (ShowSummary && !IsLoading && !HasError)
    {
        <MudCardActions Class="chart-summary">
            <div class="summary-stats d-flex justify-space-around w-100">
                @foreach (var stat in GetSummaryStats())
                {
                    <div class="summary-stat text-center">
                        <MudText Typo="Typo.h6" Color="@stat.Color">@stat.Value</MudText>
                        <MudText Typo="Typo.caption">@stat.Label</MudText>
                        @if (stat.Change.HasValue)
                        {
                            <div class="d-flex align-center justify-center mt-1">
                                <MudIcon Icon="@(stat.Change > 0 ? Icons.Material.Filled.TrendingUp : Icons.Material.Filled.TrendingDown)"
                                         Color="@(stat.Change > 0 ? Color.Success : Color.Error)"
                                         Size="Size.Small" />
                                <MudText Typo="Typo.caption" 
                                         Color="@(stat.Change > 0 ? Color.Success : Color.Error)"
                                         Class="ml-1">
                                    @(Math.Abs(stat.Change.Value).ToString("F1"))%
                                </MudText>
                            </div>
                        }
                    </div>
                }
            </div>
        </MudCardActions>
    }
</MudCard>

@code {
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public string Subtitle { get; set; } = "";
    [Parameter] public ChartType ChartType { get; set; } = ChartType.Line;
    [Parameter] public List<ChartSeries> ChartSeries { get; set; } = new();
    [Parameter] public string[] XAxisLabels { get; set; } = Array.Empty<string>();
    [Parameter] public int Height { get; set; } = 300;
    [Parameter] public bool ShowTimeRangeSelector { get; set; } = true;
    [Parameter] public bool ShowDataTable { get; set; } = false;
    [Parameter] public bool ShowSummary { get; set; } = true;
    [Parameter] public EventCallback<string> OnTimeRangeChanged { get; set; }
    [Parameter] public EventCallback OnRefresh { get; set; }
    [Parameter] public EventCallback OnExport { get; set; }

    private string SelectedTimeRange = "30d";
    private int SelectedIndex = -1;
    private bool IsLoading = false;
    private bool HasError = false;

    private ChartOptions GetChartOptions() => new()
    {
        YAxisTicks = 1000,
        MaxNumYAxisTicks = 10,
        YAxisLines = true,
        XAxisLines = false,
        LineStrokeWidth = 3,
        ChartPalette = GetChartPalette()
    };

    private string[] GetChartPalette() => new[]
    {
        "#2563eb", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6",
        "#06b6d4", "#84cc16", "#f97316", "#ec4899", "#6366f1"
    };

    private IEnumerable<ChartDataRow> GetTableData()
    {
        var rows = new List<ChartDataRow>();
        for (int i = 0; i < XAxisLabels.Length; i++)
        {
            var values = ChartSeries.Select(s => s.Data.ElementAtOrDefault(i)).ToArray();
            rows.Add(new ChartDataRow { Period = XAxisLabels[i], Values = values });
        }
        return rows;
    }

    private IEnumerable<SummaryStat> GetSummaryStats()
    {
        if (ChartSeries.Count == 0 || !ChartSeries.Any(s => s.Data.Any()))
            return Array.Empty<SummaryStat>();

        var primarySeries = ChartSeries.First();
        var total = primarySeries.Data.Sum();
        var average = primarySeries.Data.Average();
        var max = primarySeries.Data.Max();
        
        // Calculate change (compare last vs previous period)
        double? change = null;
        if (primarySeries.Data.Length >= 2)
        {
            var current = primarySeries.Data.Last();
            var previous = primarySeries.Data[^2];
            if (previous > 0)
            {
                change = ((current - previous) / previous) * 100;
            }
        }

        return new[]
        {
            new SummaryStat { Label = "Total", Value = FormatNumber(total), Color = Color.Primary },
            new SummaryStat { Label = "Average", Value = FormatNumber(average), Color = Color.Info },
            new SummaryStat { Label = "Peak", Value = FormatNumber(max), Color = Color.Success },
            new SummaryStat { Label = "Change", Value = change?.ToString("+0.0%;-0.0%") ?? "N/A", Color = GetChangeColor(change), Change = change }
        };
    }

    private string FormatNumber(double number) => number switch
    {
        >= 1000000 => $"{number / 1000000:F1}M",
        >= 1000 => $"{number / 1000:F1}K",
        _ => $"{number:N0}"
    };

    private Color GetChangeColor(double? change) => change switch
    {
        > 0 => Color.Success,
        < 0 => Color.Error,
        _ => Color.Default
    };

    private async Task RefreshData()
    {
        IsLoading = true;
        HasError = false;
        StateHasChanged();

        try
        {
            await OnRefresh.InvokeAsync();
        }
        catch
        {
            HasError = true;
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task ExportData()
    {
        await OnExport.InvokeAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (OnTimeRangeChanged.HasDelegate)
        {
            await OnTimeRangeChanged.InvokeAsync(SelectedTimeRange);
        }
    }
}

public enum ChartType
{
    Line,
    Bar,
    Donut,
    Area
}

public class ChartDataRow
{
    public string Period { get; set; } = "";
    public double[] Values { get; set; } = Array.Empty<double>();
}

public class SummaryStat
{
    public string Label { get; set; } = "";
    public string Value { get; set; } = "";
    public Color Color { get; set; } = Color.Default;
    public double? Change { get; set; }
}
```

---

## Responsive Layout Components

### 6. ResponsiveContainer Component

#### Purpose
Adaptive container that adjusts layout based on screen size and content requirements.

#### MudBlazor Implementation
```razor
@using MudBlazor

<div class="responsive-container @GetContainerClass()">
    @if (IsMobile)
    {
        <!-- Mobile Layout -->
        <div class="mobile-layout">
            @if (ShowHeader && MobileHeaderContent != null)
            {
                <div class="mobile-header">
                    @MobileHeaderContent
                </div>
            }
            
            <div class="mobile-content">
                @if (MainContent != null)
                {
                    @MainContent
                }
            </div>
            
            @if (SidebarContent != null)
            {
                <div class="mobile-sidebar">
                    @SidebarContent
                </div>
            }
            
            @if (ShowFooter && FooterContent != null)
            {
                <div class="mobile-footer">
                    @FooterContent
                </div>
            }
        </div>
    }
    else if (IsTablet)
    {
        <!-- Tablet Layout -->
        <MudGrid Class="tablet-layout" Spacing="2">
            @if (ShowHeader && HeaderContent != null)
            {
                <MudItem xs="12" Class="tablet-header">
                    @HeaderContent
                </MudItem>
            }
            
            <MudItem xs="@(SidebarContent != null ? 8 : 12)" Class="tablet-main">
                @if (MainContent != null)
                {
                    @MainContent
                }
            </MudItem>
            
            @if (SidebarContent != null)
            {
                <MudItem xs="4" Class="tablet-sidebar">
                    @SidebarContent
                </MudItem>
            }
            
            @if (ShowFooter && FooterContent != null)
            {
                <MudItem xs="12" Class="tablet-footer">
                    @FooterContent
                </MudItem>
            }
        </MudGrid>
    }
    else
    {
        <!-- Desktop Layout -->
        <div class="desktop-layout">
            @if (ShowHeader && HeaderContent != null)
            {
                <div class="desktop-header">
                    @HeaderContent
                </div>
            }
            
            <div class="desktop-body d-flex">
                @if (SidebarContent != null && SidebarPosition == SidebarPosition.Left)
                {
                    <div class="desktop-sidebar desktop-sidebar-left" style="width: @($"{SidebarWidth}px")">
                        @SidebarContent
                    </div>
                }
                
                <div class="desktop-main flex-grow-1">
                    @if (MainContent != null)
                    {
                        @MainContent
                    }
                </div>
                
                @if (SidebarContent != null && SidebarPosition == SidebarPosition.Right)
                {
                    <div class="desktop-sidebar desktop-sidebar-right" style="width: @($"{SidebarWidth}px")">
                        @SidebarContent
                    </div>
                }
            </div>
            
            @if (ShowFooter && FooterContent != null)
            {
                <div class="desktop-footer">
                    @FooterContent
                </div>
            }
        </div>
    }
</div>

@code {
    [Parameter] public RenderFragment? HeaderContent { get; set; }
    [Parameter] public RenderFragment? MobileHeaderContent { get; set; }
    [Parameter] public RenderFragment? MainContent { get; set; }
    [Parameter] public RenderFragment? SidebarContent { get; set; }
    [Parameter] public RenderFragment? FooterContent { get; set; }
    [Parameter] public bool ShowHeader { get; set; } = true;
    [Parameter] public bool ShowFooter { get; set; } = true;
    [Parameter] public SidebarPosition SidebarPosition { get; set; } = SidebarPosition.Right;
    [Parameter] public int SidebarWidth { get; set; } = 300;
    [Parameter] public string CssClass { get; set; } = "";
    [Parameter] public ContainerMaxWidth MaxWidth { get; set; } = ContainerMaxWidth.Large;
    [Parameter] public bool FullHeight { get; set; } = false;

    [Inject] private IBreakpointService BreakpointService { get; set; } = null!;

    private bool IsMobile => BreakpointService.IsDevice(Breakpoint.Xs) || BreakpointService.IsDevice(Breakpoint.Sm);
    private bool IsTablet => BreakpointService.IsDevice(Breakpoint.Md);
    private bool IsDesktop => BreakpointService.IsDevice(Breakpoint.Lg) || BreakpointService.IsDevice(Breakpoint.Xl) || BreakpointService.IsDevice(Breakpoint.Xxl);

    private string GetContainerClass()
    {
        var classes = new List<string> { CssClass };
        
        if (FullHeight)
            classes.Add("full-height");
            
        classes.Add($"max-width-{MaxWidth.ToString().ToLower()}");
        
        if (IsMobile)
            classes.Add("mobile-container");
        else if (IsTablet)
            classes.Add("tablet-container");
        else
            classes.Add("desktop-container");
            
        return string.Join(" ", classes.Where(c => !string.IsNullOrEmpty(c)));
    }
}

public enum SidebarPosition
{
    Left,
    Right
}

public enum ContainerMaxWidth
{
    Small,
    Medium,
    Large,
    ExtraLarge,
    Full
}
```

#### CSS Styles
```css
.responsive-container {
    width: 100%;
    margin: 0 auto;
}

.responsive-container.full-height {
    min-height: 100vh;
}

.responsive-container.max-width-small {
    max-width: 640px;
}

.responsive-container.max-width-medium {
    max-width: 768px;
}

.responsive-container.max-width-large {
    max-width: 1024px;
}

.responsive-container.max-width-extralarge {
    max-width: 1280px;
}

.responsive-container.max-width-full {
    max-width: 100%;
}

/* Mobile Layout */
.mobile-layout {
    display: flex;
    flex-direction: column;
    min-height: 100%;
}

.mobile-header {
    padding: 16px;
    border-bottom: 1px solid var(--mud-palette-divider);
}

.mobile-content {
    flex: 1;
    padding: 16px;
}

.mobile-sidebar {
    padding: 16px;
    background-color: var(--mud-palette-background-grey);
    border-top: 1px solid var(--mud-palette-divider);
}

.mobile-footer {
    padding: 16px;
    background-color: var(--mud-palette-background-grey);
    border-top: 1px solid var(--mud-palette-divider);
}

/* Tablet Layout */
.tablet-layout {
    min-height: 100%;
}

.tablet-header,
.tablet-footer {
    padding: 16px;
}

.tablet-main {
    padding: 16px;
}

.tablet-sidebar {
    padding: 16px;
    background-color: var(--mud-palette-background-grey);
}

/* Desktop Layout */
.desktop-layout {
    display: flex;
    flex-direction: column;
    min-height: 100%;
}

.desktop-header {
    padding: 20px 24px;
    border-bottom: 1px solid var(--mud-palette-divider);
}

.desktop-body {
    flex: 1;
    min-height: 0;
}

.desktop-main {
    padding: 24px;
    overflow-y: auto;
}

.desktop-sidebar {
    padding: 24px;
    background-color: var(--mud-palette-background-grey);
    overflow-y: auto;
}

.desktop-sidebar-left {
    border-right: 1px solid var(--mud-palette-divider);
}

.desktop-sidebar-right {
    border-left: 1px solid var(--mud-palette-divider);
}

.desktop-footer {
    padding: 20px 24px;
    background-color: var(--mud-palette-background-grey);
    border-top: 1px solid var(--mud-palette-divider);
}

/* Responsive breakpoints */
@media (max-width: 767px) {
    .responsive-container {
        padding: 0;
    }
    
    .mobile-header,
    .mobile-content,
    .mobile-sidebar,
    .mobile-footer {
        padding: 12px;
    }
}

@media (min-width: 768px) and (max-width: 1023px) {
    .tablet-header,
    .tablet-main,
    .tablet-sidebar,
    .tablet-footer {
        padding: 16px;
    }
}

@media (min-width: 1024px) {
    .desktop-header,
    .desktop-main,
    .desktop-sidebar,
    .desktop-footer {
        padding: 24px;
    }
}
```

---

## Implementation Guidelines

### Component Usage Patterns

#### 1. Package Discovery Flow
```razor
<!-- Homepage trending packages -->
<ResponsiveContainer MaxWidth="ContainerMaxWidth.Large">
    <MainContent>
        <EnhancedSearchBar OnSearch="HandleSearch" EnableVoiceSearch="true" />
        
        <div class="trending-packages mt-6">
            <MudText Typo="Typo.h5" Class="mb-4">🔥 Trending This Week</MudText>
            <MudGrid>
                @foreach (var package in TrendingPackages)
                {
                    <MudItem xs="12" sm="6" md="4">
                        <PackageCard Package="package" 
                                   IsFeatured="package.IsFeatured"
                                   ShowAIInsights="true"
                                   OnInstallClick="HandleInstall"
                                   OnDetailsClick="NavigateToDetails" />
                    </MudItem>
                }
            </MudGrid>
        </div>
    </MainContent>
</ResponsiveContainer>
```

#### 2. Search Results Layout
```razor
<!-- Search results with filtering -->
<ResponsiveContainer MaxWidth="ContainerMaxWidth.ExtraLarge">
    <MainContent>
        <EnhancedSearchBar OnSearch="HandleSearch" 
                          EnableAdvancedSearch="true"
                          Categories="PackageCategories" />
        
        <div class="search-results mt-4">
            @foreach (var package in SearchResults)
            {
                <PackageCard Package="package" 
                           ShowAIInsights="true"
                           OnInstallClick="HandleInstall"
                           OnDetailsClick="NavigateToDetails"
                           Class="mb-3" />
            }
        </div>
    </MainContent>
    
    <SidebarContent>
        <!-- Advanced filters would go here -->
    </SidebarContent>
</ResponsiveContainer>
```

#### 3. Package Detail Security Section
```razor
<!-- Package detail security tab -->
<MudTabPanel Text="Security">
    <div class="security-dashboard">
        <div class="security-header d-flex align-center justify-space-between mb-4">
            <div>
                <SecurityGradeBadge Grade="@Package.SecurityGrade" 
                                  Score="@Package.SecurityScore"
                                  DetailedReport="@Package.SecurityReport"
                                  OnDetailClick="ShowSecurityModal" />
                <TrustTierIndicator Tier="@Package.TrustTier" 
                                  ShowProgressIndicator="true"
                                  Progress="@Package.TrustProgress"
                                  Class="ml-3" />
            </div>
            <MudButton Color="Color.Primary" 
                      Variant="Variant.Outlined"
                      StartIcon="Icons.Material.Filled.Security"
                      @onclick="RequestSecurityAudit">
                Request Audit
            </MudButton>
        </div>
        
        <AnalyticsChart Title="Security Score Trend"
                       ChartType="ChartType.Line"
                       ChartSeries="SecurityTrendData"
                       ShowSummary="true"
                       Height="300" />
    </div>
</MudTabPanel>
```

#### 4. Publisher Dashboard Analytics
```razor
<!-- Publisher dashboard analytics -->
<ResponsiveContainer MaxWidth="ContainerMaxWidth.ExtraLarge" FullHeight="true">
    <MainContent>
        <MudGrid>
            <MudItem xs="12" md="8">
                <AnalyticsChart Title="Download Trends"
                               Subtitle="Package downloads over time"
                               ChartType="ChartType.Line"
                               ChartSeries="DownloadData"
                               ShowTimeRangeSelector="true"
                               ShowDataTable="false"
                               OnRefresh="RefreshDownloadData" />
            </MudItem>
            
            <MudItem xs="12" md="4">
                <AnalyticsChart Title="Security Distribution"
                               ChartType="ChartType.Donut"
                               ChartSeries="SecurityDistribution"
                               ShowSummary="false"
                               Height="300" />
            </MudItem>
            
            <MudItem xs="12">
                <div class="package-performance">
                    <MudText Typo="Typo.h6" Class="mb-3">📦 Your Packages</MudText>
                    @foreach (var package in PublisherPackages)
                    {
                        <PackageCard Package="package" 
                                   ShowAIInsights="false"
                                   OnDetailsClick="NavigateToManagement"
                                   Class="mb-3" />
                    }
                </div>
            </MudItem>
        </MudGrid>
    </MainContent>
    
    <SidebarContent>
        <!-- Publisher tools and notifications -->
        <div class="publisher-sidebar">
            <MudPaper Class="pa-4 mb-4">
                <MudText Typo="Typo.h6" Class="mb-2">🚨 Security Alerts</MudText>
                <!-- Security alerts list -->
            </MudPaper>
            
            <MudPaper Class="pa-4">
                <MudText Typo="Typo.h6" Class="mb-2">🔧 Quick Actions</MudText>
                <!-- Quick action buttons -->
            </MudPaper>
        </div>
    </SidebarContent>
</ResponsiveContainer>
```

### Accessibility Implementation

#### ARIA Labels and Descriptions
```razor
<!-- Enhanced accessibility for interactive components -->
<SecurityGradeBadge Grade="A+" 
                   Score="9.4"
                   aria-label="Security grade A+ with score 9.4 out of 10"
                   aria-describedby="security-help"
                   role="button"
                   tabindex="0" />

<div id="security-help" class="sr-only">
    This package has been thoroughly analyzed and meets the highest security standards.
    Click for detailed security report.
</div>
```

#### Keyboard Navigation
```csharp
// Add keyboard event handlers to interactive components
private async Task HandleKeyPress(KeyboardEventArgs e)
{
    if (e.Key == "Enter" || e.Key == " ")
    {
        await OnClick.InvokeAsync();
    }
    else if (e.Key == "Escape")
    {
        await OnCancel.InvokeAsync();
    }
}
```

#### Screen Reader Announcements
```csharp
// Dynamic content announcements
[Inject] private IDialogService DialogService { get; set; } = null!;

private async Task AnnounceToScreenReader(string message)
{
    // Use ARIA live regions for dynamic updates
    await InvokeAsync(() =>
    {
        // Implementation would update live region
        StateHasChanged();
    });
}
```

### Performance Optimization

#### Lazy Loading Implementation
```razor
<!-- Lazy load package cards for better performance -->
<Virtualize Items="@SearchResults" Context="package">
    <PackageCard Package="package" 
                OnInstallClick="HandleInstall"
                OnDetailsClick="NavigateToDetails" />
</Virtualize>
```

#### Component Caching
```csharp
// Implement component-level caching for expensive operations
private readonly MemoryCache _componentCache = new();

private async Task<TResult> GetCachedResult<TResult>(string key, Func<Task<TResult>> factory)
{
    if (_componentCache.TryGetValue(key, out TResult? cached))
    {
        return cached!;
    }

    var result = await factory();
    _componentCache.Set(key, result, TimeSpan.FromMinutes(5));
    return result;
}
```

---

## Conclusion

This enhanced component library specification provides comprehensive guidance for implementing MCP Hub's web application using MudBlazor. Each component is designed with:

- **Security-first approach** with prominent security indicators
- **Responsive design** that works across all devices
- **Accessibility compliance** meeting WCAG 2.1 AA standards
- **Performance optimization** through lazy loading and caching
- **Developer experience** with clear APIs and comprehensive documentation

The specifications enable the frontend development team to create a professional, enterprise-grade interface that establishes MCP Hub as the premier registry for MCP servers while maintaining excellent usability and accessibility for all users.

These components form the foundation for Phase 2B implementation and can be extended and customized as the platform evolves and grows.