using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TuringMachineClassLib
{
    /// <summary>
    /// Перечисление сторон, в которые можно расширять ленту МТ.
    /// </summary>
    public enum ExpandTo
    {
        Left,
        Right
    }

    /// <summary>
    /// Лента МТ с данными.
    /// </summary>
    public class TMTape
    {
        public TuringMachine Parent { get; set; }

        /*
         * Данные на ленте.
         * Массив имеет пустые ячейки по обеим сторонам
         * для дозаписи данных; также имеется возможность
         * его расширять методом Expand.
         */

        public char[] Data { get; private set; }

        // Максимальное число данных на ленте.

        private static int _maxDataLength = 16365; 

        // Начальное смещение данных.

        public int InitialDataOffset { get; private set; }

        /*
         * Количество ячеек, на которые лента расширяется влево.
         */

        private int _cellsExpandToLeftQuantity;
        public int CellsExpandToLeftQuantity
        {
            get
            {
                int bufCellsExpandToLeftQuantity = _cellsExpandToLeftQuantity;
                _cellsExpandToLeftQuantity = _cellsExpandToLeftQuantity >> 1;
                return bufCellsExpandToLeftQuantity;
            }
        }

        /* Количество ячеек, на которые лента расширяется вправо. */

        private int _cellsExpandToRightQuantity;
        public int CellsExpandToRightQuantity
        {
            get
            {
                int bufCellsExpandToRightQuantity = _cellsExpandToRightQuantity;
                _cellsExpandToRightQuantity = _cellsExpandToRightQuantity >> 1;
                return bufCellsExpandToRightQuantity;
            }
        }

        // Конструктор.

        public TMTape(int memSize, char[] initData, int initDataOffset)
        {
            Initialize(memSize, initData, initDataOffset);
        }

        // Конструктор, принимающий на вход конфигурацию МТ.

        public TMTape(TuringMachineConfig tmConfig)
        {
            Data = new char[tmConfig.MemorySize];

            string initData = "";

            if (tmConfig.InitialDataSource.Equals("Текст"))
                initData = tmConfig.InitialData;
            else if (tmConfig.InitialDataSource.Equals("Файл"))
            {
                using (System.IO.TextReader sourceFile = new System.IO.StreamReader(tmConfig.InitialData))
                {
                    initData = sourceFile.ReadToEnd();
                    StringBuilder sbInitData = new StringBuilder(initData);
                    for (int i = 0; i < sbInitData.Length;)
                    {
                        if (sbInitData[i] == '\n')
                            sbInitData.Remove(i, 1);
                        else
                            i++;
                    }
                    initData = sbInitData.ToString();
                }
            }

            int initDataOffset = tmConfig.InitialDataOffset,
                memSize = tmConfig.MemorySize;

            if (memSize < initData.Length + initDataOffset)
                initData = initData.Remove(memSize - initDataOffset);

            Initialize(memSize, initData.ToCharArray(), initDataOffset);
        }

        // Инициализация свойств ленты МТ.

        public void Initialize(int memSize, char[] initData, int initDataOffset)
        {
            Data = new char[memSize];

            initData.CopyTo(Data, initDataOffset);
            for (int i = 0; i < initDataOffset; i++)
                Data[i] = '_';
            for (int i = initDataOffset + initData.Length; i < Data.Length; i++)
                Data[i] = '_';

            InitialDataOffset = initDataOffset;
        }

        // Расширение массива данных.

        public void Expand(ExpandTo et)
        {
            long newLen;

            int  copyDataToBufferIndex;

            int iStart, iEnd;
            if (et == ExpandTo.Left)
            {
                newLen = CellsExpandToLeftQuantity + Data.Length;
                copyDataToBufferIndex = _cellsExpandToLeftQuantity;
                Parent.Head.Offset += _cellsExpandToLeftQuantity;
                iStart = 0;
                iEnd = copyDataToBufferIndex;
            }
            else
            {
                newLen = Data.Length + CellsExpandToRightQuantity;
                copyDataToBufferIndex = 0;
                iStart = Data.Length;
                iEnd = (int)newLen;
            }

            if (newLen > _maxDataLength)
                throw new Exception("Размер ленты превысил допустимый предел.");

            char[] buffer = new char[newLen];

            Data.CopyTo(buffer, copyDataToBufferIndex);
            for (int i = iStart; i < iEnd; i++)
                buffer[i] = '_';

            Data = buffer;
        }

        // Десериализация ленты (то есть данных).

        public static TMTape XmlDeserialize(XElement xTape)
        {
            int memSize = XmlConvert.ToInt32(xTape.Element("MemorySize").Value);

            XElement xInitData = xTape.Element("InitialData");
            string initData;
            if (xInitData.Attribute("source").Value.Equals("text"))
                initData = xTape.Element("InitialData").Value;
            else if (xInitData.Attribute("source").Value.Equals("file"))
            {
                using (System.IO.TextReader sourceFile = new System.IO.StreamReader(xInitData.Value))
                {
                    initData = sourceFile.ReadToEnd();
                    StringBuilder sbInitData = new StringBuilder(initData);
                    for (int i = 0; i < sbInitData.Length; )
                    {
                        if (sbInitData[i] == '\n' || sbInitData[i] == '\r')
                            sbInitData.Remove(i, 1);
                        else
                            i++;
                    }
                    initData = sbInitData.ToString();
                }
            }
            else
                throw new Exception("Источник начальных данных на ленте не распознан.");

            int initDataOffset = XmlConvert.ToInt32(xTape.Element("InitialDataOffset").Value);

            if (memSize < initData.Length + initDataOffset)
                initData = initData.Remove(memSize - initDataOffset);

            TMTape newTape = new TMTape
            (
                memSize,
                initData.ToCharArray(),
                initDataOffset
            );

            return newTape;
        }

        public XElement XSerialize(string source, string data = "")
        {
            if (source == "text")
                data = new string(Data);

            return new XElement("TMTape",
                new XElement("MemorySize") { Value = Data.Length.ToString() },
                new XElement("InitialData", new XAttribute("source", source)) { Value = data },
                new XElement("InitialDataOffset") { Value = "0" });
        }
    }
}
