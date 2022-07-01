using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace TuringMachineClassLib
{
    /// <summary>
    /// Класс головки машины Тьюринга.
    /// </summary>

    public class TMHead
    {
        // Смещение на ленте МТ.

        private int _offset;
        public int Offset
        {
            get { return _offset; }
            set
            {
                if (value != _offset)
                {
                    //DifferenceOffset += value - _offset;
                    _offset = value;
                }
            }
        }

        // Разница в смещении на ленте и ОТОБРАЖАЕМЫМ смещением (для визуалиации)
        // По центру ленты на экране интерфейса.

        public int DifferenceOffset { get; set; }

        // Конструктор.

        public TMHead(int offsetFromData, int dataOffset)
        {
            _offset = dataOffset + offsetFromData;

            // Отображаемое смещение сначала всегда равно нулю.
            // Позднее ОС = Offset - DifferenceOffset.

            //DifferenceOffset = 0;
            DifferenceOffset = _offset;
        }

        // Десериализация из хмл-файла.
        // Смещение идёт от начала ленты.

        public static TMHead XmlDeserialize(XElement xHead)
        {
            return new TMHead(XmlConvert.ToInt32(xHead.Attribute("OffsetFromData").Value), 0);
        }

        // Сериализация в хмл-файл.

        public XElement XSerialize()
        {
            return new XElement("TMHead", new XAttribute("OffsetFromData", Offset));
        }
    }
}
