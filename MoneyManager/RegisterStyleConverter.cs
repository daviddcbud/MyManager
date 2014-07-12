using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MoneyManager
{
    public class RegisterStyleConverter : IMultiValueConverter 
    {


        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FrameworkElement targetObject = values[0] as FrameworkElement;
            if (targetObject == null)
            {
                return DependencyProperty.UnsetValue;
            }
            var iscleared = (bool?)values[1];
            if (!iscleared.HasValue || !iscleared.Value)
            {
                return targetObject.TryFindResource("clearedStyle");
            }
            else
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
