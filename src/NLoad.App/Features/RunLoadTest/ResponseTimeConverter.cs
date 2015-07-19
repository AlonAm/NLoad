using System;
using System.Globalization;
using System.Windows.Data;

namespace NLoad.App.Features.RunLoadTest
{
    public class ResponseTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan)) return value.ToString();

            var elapsed = (TimeSpan)value;

            return string.Format("{0:mm\\:ss\\.fff}", elapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}