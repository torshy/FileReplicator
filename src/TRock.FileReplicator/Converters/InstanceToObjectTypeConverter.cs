using System;
using System.Globalization;
using System.Windows.Data;

namespace TRock.FileReplicator.Converters
{
    public class InstanceToObjectTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            return value.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}