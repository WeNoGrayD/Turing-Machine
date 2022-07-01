using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringMachineClassLib
{
    /// <summary>
    /// Класс, который содержит все необходимые параметры
    /// для создания экземпляра машины Тьюринга.
    /// </summary>

    public class TuringMachineConfig
    { 
        // Алфавит, используемый МТ. Неявно включает пробел (нижнее подчёркивание).

        public string Alphabet { get; set; }

        // Параметры ленты.

        // Размер начальной памяти.

        public int MemorySize { get; set; }

        // Источник начальных данных (файл или сам текст)

        public string InitialDataSource { get; set; }

        // Начальные данные.

        public string InitialData { get; set; }

        // Смещение начальных данных на ленте.

        public int InitialDataOffset { get; set; }

        // Головка МТ.
        
        public int HeadOffset { get; set; }

        // Конструктор.

        public TuringMachineConfig()
        {
            Alphabet = "";
            MemorySize = 0;
            InitialDataSource = "Текст";
            InitialData = "";
            InitialDataOffset = 0;
            HeadOffset = -1;
        }
    }
}
