using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TuringMachineViewModelComponentsLib
{
    // Класс содержит правила валидации вводимых чисел. Числа должны быть натуральными.

    public class NaturalNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int natNum = 0;
            string errMes = "";
            if (!int.TryParse((string)value, out natNum))
            {
                errMes = "Введены недопустимые символы.\nПожалуйста, введите число.";
                return new ValidationResult(false, errMes);
            }

            if (natNum < 0)
            {
                errMes = "Значение параметра должно быть больше либо равно нулю.";
                return new ValidationResult(false, errMes);
            }

            return new ValidationResult(true, null);
        }
    }
}
