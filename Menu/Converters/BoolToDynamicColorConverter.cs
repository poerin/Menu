using System.Globalization;

namespace Menu.Converters;

public class BoolToDynamicColorConverter : IValueConverter
{
    public required Color LightTrueColor { get; set; }
    public required Color DarkTrueColor { get; set; }
    public required Color LightFalseColor { get; set; }
    public required Color DarkFalseColor { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var currentTheme = Application.Current?.RequestedTheme;

        if (value == null || (bool)value)
        {
            return currentTheme == AppTheme.Light ? LightTrueColor : DarkTrueColor;
        }
        else
        {
            return currentTheme == AppTheme.Light ? LightFalseColor : DarkFalseColor;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

