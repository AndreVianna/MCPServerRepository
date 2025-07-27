# MCP Hub Design System Specification

## Executive Summary

This document defines the comprehensive design system for MCP Hub, a security-focused MCP (Model Context Protocol) server registry platform. The design emphasizes trust, security, and professional enterprise-grade aesthetics while maintaining excellent usability for developers.

## Design Philosophy

### Core Principles
- **Security First**: All design decisions prioritize security transparency and trust
- **Professional Excellence**: Enterprise-grade aesthetics that instill confidence
- **Developer-Centric**: Optimized for developer workflows and package discovery
- **Performance Focused**: Fast, responsive, and efficient user experience
- **Accessibility Compliant**: WCAG 2.1 AA compliance throughout

### Brand Positioning
MCP Hub positions itself as the trusted, secure, and professional registry for MCP servers - the "enterprise-grade npm for MCP packages" with uncompromising security standards.

## Color System

### Primary Palette
```
Primary Blue: #2563eb     (MCP Hub Brand Blue)
Primary Dark: #1e3a8a     (Dark Blue for depth)
Primary Light: #93c5fd    (Light Blue for accents)
```

### Security Status Colors
```
Security Grade A: #10b981  (Emerald Green - Excellent)
Security Grade B: #f59e0b  (Amber - Good)
Security Grade C: #ef4444  (Red - Needs Attention)
Security Grade F: #7f1d1d  (Dark Red - Critical)
```

### Trust Tier Colors
```
Unverified:   #6b7280     (Gray)
Verified:     #2563eb     (Primary Blue)
Trusted:      #10b981     (Green)
Enterprise:   #7c3aed     (Purple)
```

### Neutral Palette
```
Gray 50:  #f9fafb
Gray 100: #f3f4f6
Gray 200: #e5e7eb
Gray 300: #d1d5db
Gray 400: #9ca3af
Gray 500: #6b7280
Gray 600: #4b5563
Gray 700: #374151
Gray 800: #1f2937
Gray 900: #111827
```

### Semantic Colors
```
Success: #10b981
Warning: #f59e0b
Error:   #ef4444
Info:    #3b82f6
```

## Typography

### Font Family
Primary: `Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif`
Monospace: `"Fira Code", "JetBrains Mono", Consolas, monospace`

### Type Scale
```
Display Large:  36px / 40px (Font Weight: 700)
Display Medium: 30px / 36px (Font Weight: 700)
Display Small:  24px / 32px (Font Weight: 600)

Headline Large:  20px / 28px (Font Weight: 600)
Headline Medium: 18px / 24px (Font Weight: 600)
Headline Small:  16px / 24px (Font Weight: 600)

Body Large:     16px / 24px (Font Weight: 400)
Body Medium:    14px / 20px (Font Weight: 400)
Body Small:     12px / 16px (Font Weight: 400)

Label Large:    14px / 20px (Font Weight: 500)
Label Medium:   12px / 16px (Font Weight: 500)
Label Small:    11px / 16px (Font Weight: 500)
```

## Spacing System

### Base Unit: 4px

```
xs:  4px
sm:  8px
md:  12px
lg:  16px
xl:  20px
2xl: 24px
3xl: 32px
4xl: 40px
5xl: 48px
6xl: 64px
7xl: 80px
8xl: 96px
```

## Component Library

### Package Card Component
```
Structure:
- Header: Package name, version, trust tier badge
- Body: Description, security score, download stats
- Footer: Tags, last updated, publisher info

Variants:
- List view (horizontal layout)
- Grid view (vertical layout)
- Featured (larger with more details)

States:
- Default, Hover, Selected, Disabled
```

### Security Score Badge
```
Visual Design:
- Circular progress indicator
- Letter grade (A+, A, B, C, F)
- Color-coded background
- Tooltip with detailed breakdown

Sizes: Small (24px), Medium (32px), Large (48px)
```

### Trust Tier Badge
```
Design:
- Pill-shaped badge
- Icon + text combination
- Color-coded by tier level
- Consistent sizing across contexts

Tiers:
- Unverified (Gray, no icon)
- Verified (Blue, checkmark)
- Trusted (Green, shield)
- Enterprise (Purple, crown)
```

### Search Components
```
Search Bar:
- Large, prominent placement
- Real-time suggestions
- Category filtering
- Voice search icon

Filter Sidebar:
- Collapsible sections
- Multi-select with counts
- Clear all functionality
- Applied filters display
```

## Layout System

### Grid System
- 12-column grid
- Responsive breakpoints: 320px, 768px, 1024px, 1440px
- Container max-width: 1200px
- Gutter: 24px (desktop), 16px (mobile)

### Responsive Breakpoints
```
Mobile:     320px - 767px
Tablet:     768px - 1023px
Desktop:    1024px - 1439px
Large:      1440px+
```

## Page Layouts

### Homepage Layout
```
Header Navigation (64px height)
├── Logo + Brand
├── Search Bar (central)
├── Navigation Links
└── User Account

Hero Section (400px height)
├── Main Headline
├── Subtitle
├── Primary Search Bar
└── Statistics Counter

Trending Packages (auto height)
├── Section Header
├── Package Grid (3x2 on desktop)
└── View All Link

Categories Section (300px)
├── Category Grid (4 columns)
└── Browse All Link

Security Messaging (200px)
├── Security Promise
├── Trust Metrics
└── Learn More CTA

Footer (200px)
├── Links
├── Resources
└── Legal
```

### Search Results Layout
```
Header Navigation (64px)

Search Area (120px)
├── Search Bar + Filters Toggle
├── Results Count
└── Sort Options

Main Content (flexible)
├── Filter Sidebar (280px width)
│   ├── Categories
│   ├── Trust Tiers
│   ├── Security Scores
│   └── Additional Filters
└── Results Grid (flexible)
    ├── Package Cards
    ├── Pagination
    └── Results Per Page
```

### Package Detail Layout
```
Header Navigation (64px)

Package Header (200px)
├── Package Info
├── Installation Commands
├── Security Score
└── Trust Tier Badge

Tabbed Content (flexible)
├── Tab Navigation
├── Overview Tab
│   ├── README content
│   ├── Dependencies
│   └── Usage Examples
├── Security Tab
│   ├── Security Report Card
│   ├── Vulnerability Details
│   └── Scan History
└── Installation Tab
    ├── CLI Commands
    ├── Configuration Examples
    └── Integration Guides

Sidebar (300px)
├── Publisher Info
├── Download Stats
├── Version History
└── Related Packages
```

### Publisher Dashboard Layout
```
Header Navigation (64px)

Dashboard Header (120px)
├── Welcome Message
├── Quick Stats
└── New Package CTA

Main Dashboard (flexible)
├── Analytics Overview
│   ├── Download Charts
│   ├── Security Status
│   └── Performance Metrics
├── Package Management
│   ├── Package List
│   ├── Security Alerts
│   └── Version Management
└── Account Settings
    ├── Profile Management
    ├── API Keys
    └── Notification Preferences
```

## Interactive States

### Button States
```
Primary Buttons:
- Default: #2563eb background, white text
- Hover: #1d4ed8 background
- Active: #1e40af background
- Disabled: #9ca3af background
- Focus: 2px ring #93c5fd

Secondary Buttons:
- Default: transparent background, #2563eb border/text
- Hover: #f8fafc background
- Active: #f1f5f9 background
```

### Form Elements
```
Input Fields:
- Default: #f9fafb background, #d1d5db border
- Focus: #ffffff background, #2563eb border, ring
- Error: #fef2f2 background, #ef4444 border
- Success: #f0fdf4 background, #10b981 border
```

## Animation Guidelines

### Transitions
```
Default Duration: 150ms
Easing: ease-out
Properties: opacity, transform, background-color, border-color

Page Transitions: 300ms
Loading States: 200ms pulsing animation
Micro-interactions: 100ms
```

### Loading States
```
Skeleton Loading:
- Gray shimmer animation
- Matches content structure
- 1.5s animation cycle

Spinner:
- Primary blue color
- 24px default size
- 1s rotation cycle
```

## Accessibility Standards

### WCAG 2.1 AA Compliance
- Minimum contrast ratio: 4.5:1 for normal text
- Minimum contrast ratio: 3:1 for large text
- Focus indicators clearly visible
- Keyboard navigation support
- Screen reader optimization

### Keyboard Navigation
```
Tab Order: Logical flow top to bottom, left to right
Skip Links: Available for main content
Focus Traps: Modal dialogs and overlays
Escape Key: Closes modals and dropdowns
```

## Icon System

### Icon Library
Primary: Heroicons v2 (Outline and Solid variants)
Secondary: Lucide React for specialized icons

### Icon Sizes
```
xs: 12px (inline with small text)
sm: 16px (inline with body text)
md: 20px (standalone icons)
lg: 24px (prominent actions)
xl: 32px (feature icons)
```

### Security Icons
```
Shield Check: Trusted packages
Exclamation Triangle: Warnings
Lock Closed: Secure/Private
Eye: Transparency/Visibility
Bug: Security vulnerabilities
```

## Brand Assets

### Logo Usage
- Minimum size: 24px height
- Clear space: 1x logo height on all sides
- Color variations: Full color, monochrome, reverse

### Brand Colors in Context
- Primary blue for CTAs and key actions
- Security colors only for security-related information
- Neutral grays for supporting content
- Trust tier colors only for tier indicators

## Implementation Notes

### MudBlazor Component Mapping
```
MudCard → Package Cards
MudDataGrid → Package Lists
MudTabs → Package Detail Tabs
MudChip → Tags and Badges
MudButton → All button variants
MudTextField → Search and forms
MudSelect → Filters and dropdowns
MudProgressCircular → Security scores
```

### CSS Custom Properties
```css
:root {
  --mcp-primary: #2563eb;
  --mcp-primary-dark: #1e3a8a;
  --mcp-primary-light: #93c5fd;
  
  --mcp-security-a: #10b981;
  --mcp-security-b: #f59e0b;
  --mcp-security-c: #ef4444;
  --mcp-security-f: #7f1d1d;
  
  --mcp-trust-unverified: #6b7280;
  --mcp-trust-verified: #2563eb;
  --mcp-trust-trusted: #10b981;
  --mcp-trust-enterprise: #7c3aed;
  
  --mcp-spacing-xs: 4px;
  --mcp-spacing-sm: 8px;
  --mcp-spacing-md: 12px;
  --mcp-spacing-lg: 16px;
  --mcp-spacing-xl: 20px;
  
  --mcp-font-family: Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
  --mcp-font-mono: "Fira Code", "JetBrains Mono", Consolas, monospace;
}
```

## Design Tokens

### Semantic Tokens
```
Button Primary Background: var(--mcp-primary)
Button Primary Text: #ffffff
Button Secondary Background: transparent
Button Secondary Border: var(--mcp-primary)
Button Secondary Text: var(--mcp-primary)

Card Background: #ffffff
Card Border: #e5e7eb
Card Shadow: 0 1px 3px rgba(0, 0, 0, 0.1)

Text Primary: #111827
Text Secondary: #6b7280
Text Disabled: #9ca3af
```

This design system provides a comprehensive foundation for creating consistent, professional, and security-focused UI components for the MCP Hub platform.