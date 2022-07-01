using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace TuringMachineClassLib
{
    /// <summary>
    /// Перечисление возможных передвижений по ячейкам
    /// ленты машины Тьюринга.
    /// </summary>

    [Serializable]
    public enum TransitionMove
    {
        ToLeftCell = -1,
        StayDown = 0,
        ToRightCell = 1
    }

    /// <summary>
    /// Запись о связанном состоянии
    /// (состоянии, в которое имеется переход из данного).
    /// </summary>

    [Serializable]
    public class TransitionRecord : IEquatable<TransitionRecord>
    {
        public char CharToWrite { get; set; }

        public TransitionMove Move { get; set; }

        public string DestTMState { get; set; }

        public TransitionRecord(char charWrite, TransitionMove move, string destState)
        {
            CharToWrite = charWrite;
            Move = move;
            DestTMState = destState;
        }

        public TransitionRecord(char charWrite, string moveStr, string destState)
        {
            CharToWrite = charWrite;

            Move = TransitionRecord.ArrowNotationToEnum(moveStr);

            DestTMState = destState;
        }

        // Перевод действия на ленте из стрелочной нотации в перечисление.

        public static TransitionMove ArrowNotationToEnum(string arrow)
        {
            switch (arrow)
            {
                case "<-": { return TransitionMove.ToLeftCell; }
                case "...": default: { return TransitionMove.StayDown; }
                case "->": { return TransitionMove.ToRightCell; }
            }
        }

        // Перевод действия на ленте из из перечисления в стрелочную нотацию.

        public static string EnumToArrowNotation(TransitionMove move)
        {
            switch (move)
            {
                case TransitionMove.ToLeftCell: { return "<-"; }
                case TransitionMove.StayDown: default: { return "..."; }
                case TransitionMove.ToRightCell: { return "->"; }
            }
        }

        // Сравнение двух записей.

        public bool Equals(TransitionRecord other)
        {
            if (this.CharToWrite == other.CharToWrite)
                return true;
            return false;
        }

        // Сериализация записи таблицы переходов.

        static public XElement XSerialize(char cond, TransitionRecord tRecord)
		{
			string moveArrowNotation = "";
			switch (tRecord.Move)
			{
				case TransitionMove.ToLeftCell: { moveArrowNotation = "<-"; break; }
				case TransitionMove.StayDown: { moveArrowNotation = "..."; break; }
				case TransitionMove.ToRightCell: { moveArrowNotation = "->"; break; }
			}
			
			XElement xTRecord = new XElement("TransitionRecord");
            xTRecord.Add(new XElement("Condition") { Value = cond.ToString() });
            xTRecord.Add(new XElement("CharToWrite") { Value = tRecord.CharToWrite.ToString() });
			xTRecord.Add(new XElement("Move") { Value = moveArrowNotation });
			xTRecord.Add(new XElement("DestTMState") { Value = tRecord.DestTMState });
			
			return xTRecord;
		}
		
		// Десериализация записи таблицы переходов.
		
		static public TransitionRecord XDeserialize(XElement xTRecord)
		{
			char charWrite = XmlConvert
				.ToChar(xTRecord.Element("CharToWrite").Value);
			string moveStr = xTRecord.Element("Move").Value,
				   destState = xTRecord.Element("DestTMState").Value;
				   
			return new TransitionRecord(charWrite, moveStr, destState);
		}
    }

    /// <summary>
    /// Перечисление статусов состояния:
    /// -- допускающее;
    /// -- завершающее;
    /// -- запретное.
    /// </summary>

    [Serializable]
    public enum TMStateStatus
    {
        Rejecting = -1,
        Start = 0,
        Usual = 1,
        Accepting = 2
    }

    /// <summary>
    /// Состояние машины Тьюринга.
    /// </summary>

    [Serializable]
    public class TMState : IEquatable<TMState>
    {
        public string Name { get; set; }

        public TMStateStatus Status { get; set; }
		
		// Столбец в таблице переходов.

        public ObservableDictionary<char, TransitionRecord> TransitionsColumn { get; set; }

        public string Comment { get; set; }

        // Конструктор.

        public TMState()
        {
			TransitionsColumn = new ObservableDictionary<char, TransitionRecord>();
        }

        /*
         * Создание состояния-пустышки с:
         *   -- именем;
         *   -- таблицей переходов, заполненной пустыми записями
         *      (т.е. указывающими на это же состояние с записью пробела 
         *       и стоянием на месте)
         */

        public static TMState CreateEmpty(string name, TuringMachine tm)
        {
            ObservableDictionary<char, TransitionRecord> transitionsColumn = new
                ObservableDictionary<char, TransitionRecord>();
            foreach (char a in tm.Alphabet)
                transitionsColumn.Add(a, new TransitionRecord('_', "...", name));

            TMState newState = new TMState()
            {
                Name = name,
                TransitionsColumn = transitionsColumn,
                Comment = ""
            };
            newState.SetStatus("Usual");

            return newState;
        }

        public void SetStatus(string statusStr)
        {
            switch (statusStr)
            {
                case "Rejecting": { Status = TMStateStatus.Rejecting; break; }
				case "Start": { Status = TMStateStatus.Start; break; }
                case "Accepting": { Status = TMStateStatus.Accepting; break; }
                case "Usual": { Status = TMStateStatus.Usual; break; }
            }
        }

        // Десериализация состояния.

        static public TMState XDeserialize(XElement xState, TuringMachine tm)
        {
            string name = xState.Attribute("Name").Value;

            Dictionary<char, TransitionRecord> transitionsColumnBuf =
                (
                    from tRecord in xState.Element("TransitionsTable")
                                          .Elements("TransitionRecord")
                     select new KeyValuePair<char, TransitionRecord>
                     (
                         XmlConvert.ToChar(tRecord.Element("Condition")
                                                  .Value),
                                           TransitionRecord.XDeserialize(tRecord)
                     )
                )
                .ToDictionary(r => r.Key, r => r.Value);

            foreach (char a in tm.Alphabet)
            {
                if (!transitionsColumnBuf.ContainsKey(a))
                    transitionsColumnBuf.Add(a, new TransitionRecord('_', "...", name));
            }

            // Сортировка записей в столбце по порядку символов в алфавите.

            ObservableDictionary<char, TransitionRecord> transitionsColumn = new
                ObservableDictionary<char, TransitionRecord>();
            foreach (char a in tm.Alphabet)
                transitionsColumn.Add(a, transitionsColumnBuf[a]);

            TMState newState = new TMState()
            {
                Name = name,
                TransitionsColumn = transitionsColumn, 
                Comment = xState.Element("Comment").Value
            };

            newState.SetStatus(statusStr: xState.Element("Status").Value);

            return newState;
        }
		
		// Сериализация.
		
		public XElement XSerialize()
		{
			string status = "";
			switch (Status)
			{
				case TMStateStatus.Rejecting: { status = "Rejecting"; break; }
				case TMStateStatus.Start: { status = "Start"; break; }
				case TMStateStatus.Accepting: { status = "Accepting"; break; }
				case TMStateStatus.Usual: { status = "Usual"; break; }
			}
			
			XElement xState = new XElement("TMState");
			xState.Add(new XAttribute("Name", Name));
			xState.Add(new XElement("Status") { Value = status });
			
			IEnumerable<XElement> xTransitionRecords =
				from tRecord in TransitionsColumn
				select TransitionRecord.XSerialize(tRecord.Key, tRecord.Value);

            XElement xTransitionsColumn = new XElement("TransitionsTable");

            foreach(XElement xTransitionRecord in xTransitionRecords)
			    xTransitionsColumn.Add(xTransitionRecord);

            xState.Add(xTransitionsColumn);
			xState.Add(new XElement("Comment") { Value = Comment });
			
			return xState;
		}

        // Реализация интерфейса IEquatable

        public bool Equals(TMState other)
        {
            return this.Name == other.Name;
        }
    }
}
