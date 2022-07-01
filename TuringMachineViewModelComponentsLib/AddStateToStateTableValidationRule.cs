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
    public class AddStateToStateTableValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string addStateName = (string)value;

            if (addStateName == "")
                return new ValidationResult(true, null);

            string errMes = "";

            if (TuringMachineViewModel.TM.States.ContainsKey(addStateName))
            {
                errMes = $"Состояние с названием {addStateName} уже существует в таблице состояний. Пожалуйста, выберите другое.";
                return new ValidationResult(false, errMes);
            }

            return new ValidationResult(true, null);
        }
    }
}
