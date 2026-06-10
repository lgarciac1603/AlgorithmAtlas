using MudBlazor;

namespace AlgorithmAtlas.Theme;

public static class AppTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#818CF8",
            Secondary = "#22D3EE",
            Background = "#0B0E14",
            Surface = "#11151F",
            AppbarBackground = "#11151F",
            DrawerBackground = "#0B0E14",
            TextPrimary = "#E5E7EB",
            TextSecondary = "#9CA3AF",
            ActionDefault = "#9CA3AF",
            LinesDefault = "#1F2937",
            Divider = "#1F2937",
            DrawerText = "#9CA3AF",
            Success = "#34D399",
            Warning = "#FBBF24",
            Error = "#F87171"
        },
        PaletteLight = new PaletteLight
        {
            Primary = "#6366F1",
            Secondary = "#0891B2",
            Background = "#F8FAFC",
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            TextPrimary = "#111827",
            TextSecondary = "#4B5563"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Inter", "ui-sans-serif", "system-ui", "sans-serif"]
            },
            H1 = new H1Typography { FontFamily = ["Inter", "sans-serif"], FontWeight = "800" },
            H2 = new H2Typography { FontFamily = ["Inter", "sans-serif"], FontWeight = "700" },
            H3 = new H3Typography { FontFamily = ["Inter", "sans-serif"], FontWeight = "700" },
            H4 = new H4Typography { FontFamily = ["Inter", "sans-serif"], FontWeight = "600" },
            H5 = new H5Typography { FontFamily = ["Inter", "sans-serif"], FontWeight = "600" },
            H6 = new H6Typography { FontFamily = ["Inter", "sans-serif"], FontWeight = "600" }
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px"
        }
    };
}
