using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Converters
{
    class RowColorConverter : IValueConverter
    {
        public Color EvenColor { get; set; } = Colors.White;
        public Color OddColor { get; set; } = Color.FromArgb("#F8F9FA");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEven)
                return isEven ? EvenColor : OddColor;

            return EvenColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
