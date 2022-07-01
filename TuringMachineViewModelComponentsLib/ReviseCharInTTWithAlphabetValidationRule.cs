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
    public class ReviseCharInTTWithAlphabetValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string alphaChar = (string)value;
            string errMes = "";

            if (alphaChar.Length != 1)
            {
                errMes = "Алфавит должен состоять из одиночных символов. Пожалуйста, измените свой выбор.";
                return new ValidationResult(false, errMes);
            }

            // Если символ в алфавите НЕ ВСТРЕЧАЕТСЯ, то это ошибка.
            List<char> uselessChars = new List<char>();
            if (!TuringMachine.ReviseDataWithAlphabet(TuringMachineViewModel.TM.Alphabet.ToArray(),
                alphaChar.ToCharArray(), out uselessChars))
            {
                errMes = $"Алфавит не содержит символ '{uselessChars[0]}'. Пожалуйста, измените свой выбор.";
                return new ValidationResult(false, errMes);
            }

            return new ValidationResult(true, null);
        }
    }
}
