using System;
using Microsoft.UI.Xaml.Data;
using RtspViewer.Configuration;

namespace RtspViewer.WinUi.Converters
{
    public class EnumToIntConverter<TEnum> : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Enum.Parse(typeof(TEnum), value.ToString());
        }
    }

    public class ConnectionTypeConverter : EnumToIntConverter<ConnectionType> { }
    public class StreamScaleConverter : EnumToIntConverter<StreamScale> { }
}
