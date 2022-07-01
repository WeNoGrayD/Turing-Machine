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
    public class TransitionMoveConverter : IValueConverter, IMultiValueConverter
    {
        private TransitionMove _move;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _move = (TransitionMove)value;
            return TransitionRecord.EnumToArrowNotation((TransitionMove)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return TransitionRecord.ArrowNotationToEnum((string)value);
            else
                return _move;
        }

        private TransitionRecord _tRecord;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            _tRecord = (TransitionRecord)values[0];
            return TransitionRecord.EnumToArrowNotation((TransitionMove)_move);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value != null)
                return new object[] { null, TransitionRecord.ArrowNotationToEnum((string)((ComboBoxItem)value).Content) };
            else
                return new object[] { null, _tRecord.Move };
        }
    }
}
