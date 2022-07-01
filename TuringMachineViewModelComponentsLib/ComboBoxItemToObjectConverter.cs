using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using TuringMachineClassLib;
using System.Windows.Controls;

namespace TuringMachineViewModelComponentsLib
{
    public class ComboBoxItemToObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ComboBoxItem)value)?.Content;
        }
    }
}
