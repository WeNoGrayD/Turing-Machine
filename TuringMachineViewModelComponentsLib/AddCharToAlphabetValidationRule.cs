using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Controls;
using TuringMachineClassLib;

namespace TuringMachineViewModelComponentsLib
{
    public class AddCharToAlphabetValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string addChar = (string)value;

            if (addChar.Length == 0)
                return new ValidationResult(true, null);

            string errMes = "";

            if (addChar.Length > 1)
            {
                errMes = "Алфавит должен состоять из одиночных символов. Пожалуйста, измените свой выбор.";
                return new ValidationResult(false, errMes);
            }

            if (addChar[0] == '_')
            {
                errMes = "Алфавит уже включает пробел ('_'). Пожалуйста. измените свой выбор";
                return new ValidationResult(false, errMes);
            }

            // Если символ в алфавите ВСТРЕЧАЕТСЯ, то это ошибка.
            List<char> uselessChars = new List<char>();
            if (TuringMachine.ReviseDataWithAlphabet(TuringMachineViewModel.TM.Alphabet.ToArray(),
                addChar.ToCharArray(), out uselessChars))
            {
                errMes = "Алфавит уже включает этот символ. Пожалуйста, измените свой выбор.";
                return new ValidationResult(false, errMes);
            }

            return new ValidationResult(true, null);
        }
    }
}
