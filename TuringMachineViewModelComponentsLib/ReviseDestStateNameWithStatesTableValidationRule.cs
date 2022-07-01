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
    public class ReviseDestStateNameWithStatesTableValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string destStateName = (string)value;

            if (destStateName == "")
                return new ValidationResult(true, null);

            string errMes = "";
            
            if (!TuringMachineViewModel.TM.States.ContainsKey(destStateName))
            {
                errMes = $"Состояние с названием {destStateName} отсутствует в таблице состояний." + 
                          " Пожалуйста, измените свой выбор.";
                return new ValidationResult(false, errMes);
            }

            return new ValidationResult(true, null);
        }
    }
}
