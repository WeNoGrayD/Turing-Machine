using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringMachineClassLib;

namespace TuringMachineViewModelComponentsLib
{
    public class TuringMachineViewModel
    {
        public static TuringMachine TM { get; set; }

        public TuringMachineViewModel(TuringMachine tm)
        {
            TM = tm;
        }
    }
}
