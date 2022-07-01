using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace TuringMachineClassLib
{
    /// <summary>
    /// Модель машины Тьюринга.
    /// </summary>

    public class TuringMachine
    {
        /// <summary>
        /// Формулировка задачи.
        /// </summary>

        public string ProblemFormulation { get; set; }

        /// <summary>
        /// Лента машины.
        /// </summary>

        public TMTape Tape { get; set; }

        /// <summary>
        /// Головка на ленте машины.
        /// </summary>

        public TMHead Head { get; set; }

        /// <summary>
        /// Словарь состояний, связанных с их именами (идентификаторами).
        /// </summary>

        public ObservableDictionary<string, TMState> States { get; set; }

        /// <summary>
        /// Алфавит машины.
        /// </summary>

        public ObservableCollection<char> Alphabet { get; set; }

        /// <summary>
        /// Имя (идентификатор) текущего состояния машины.
        /// </summary>

        public string CurrentStateName { get; set; }

        public TuringMachine()
        {
            Alphabet = new ObservableCollection<char>();
            Alphabet.CollectionChanged += (s, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    if (States != null)
                        foreach (TMState st in States.Values)
                        {
                            foreach (char a in e.NewItems)
                                st.TransitionsColumn.Add(a, new TransitionRecord
                                    ('_', TransitionMove.StayDown, st.Name));
                        }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    if (States != null)
                        foreach (TMState st in States.Values)
                        {
                            foreach (char a in e.OldItems)
                                st.TransitionsColumn.Remove(a);
                        }
                }
            };
        }

        /// <summary>
        /// Конструктор, принимающий на вход хэлемент
        /// </summary>
        /// <param name="xTM"></param>

        public TuringMachine(XDocument xTM) : this()
        {
			XElement xConfig = xTM.Element("TMConfig");
            ProblemFormulation = xConfig.Element("ProblemFormulation").Value;
            if (Alphabet != null)
                Alphabet.Clear();
            foreach (char a in XmlDeserializeAlphabet(xConfig.Element("Alphabet")))
                Alphabet.Add(a);
            Tape = TMTape.XmlDeserialize(xConfig.Element("TMTape")); Tape.Parent = this;
            Head = TMHead.XmlDeserialize(xConfig.Element("TMHead"));
            StatesTable st = XmlDeserializeStates(xConfig.Element("TMStates"));
            CurrentStateName = st.StartState;
            States = new ObservableDictionary<string, TMState>(st.Records); 
        }

        /// <summary>
        /// Конструктор, принимающий на вход конфигурацию МТ.
        /// </summary>
        /// <param name="tmConfig"></param>

        public TuringMachine(TuringMachineConfig tmConfig) : this()
        {
            ProblemFormulation = "";
            if (Alphabet != null)
                Alphabet.Clear();
            Alphabet.Add('_');
            foreach(char a in tmConfig.Alphabet)
                Alphabet.Add(a);
            Tape = new TMTape(tmConfig) { Parent = this };
            Head = new TMHead(tmConfig.HeadOffset, Tape.InitialDataOffset);
            if (States != null)
                States.Clear();
            else
                States = new ObservableDictionary<string, TMState>();

            TMState startState = TMState.CreateEmpty("Qs", this);

            States.Add(startState.Name, startState);
            CurrentStateName = startState.Name;
        }

        // Подписка на событие добавления символа в алфавит.

        private void SubscribeOnAlphabetEvents()
        {

        }

        /// <summary>
        /// Десериализация алфавита машины (списка символов).
        /// </summary>
        /// <param name="xAB"></param>
        /// <returns></returns>

        public static char[] XmlDeserializeAlphabet(XElement xAB)
        {
            return xAB.Value.ToCharArray();
        }

        /// <summary>
        /// Структура, представляющая таблицу состояний и имя начального состояния.
        /// </summary>

        public struct StatesTable
        {
            public string StartState { get; private set; } 
            
            public Dictionary<string, TMState> Records { get; private set; } 

            public StatesTable(string startSt, Dictionary<string, TMState> records)
            {
                StartState = startSt;
                Records = records;
            }
        }

        /// <summary>
        /// Десериализация таблицы состояний.
        /// </summary>
        /// <param name="xStates">Элемент в документе .xml, представляющий таблицу состояний.</param>
        /// <returns>
        /// Возвращает структуру, представляющую словарь состояний и имя начального состояния.
        /// </returns>

        public StatesTable XmlDeserializeStates(XElement xStates)
        {
            Dictionary<string, TMState> statesTable = 
            (
                from xState in xStates.Elements("TMState")
                let state = TMState.XDeserialize(xState, this)
                select new KeyValuePair<string, TMState>(state.Name, state)
            ).ToDictionary(s => s.Key, s => s.Value);
			
			TMState[] startStates = statesTable.Values.Where(st => st.Status == TMStateStatus.Start)
                                                      .ToArray();
			if (startStates.Length == 0)
				throw new Exception("Необходимо указать начальное состояние.");
            if (startStates.Length > 1)
                throw new Exception("Слишком много стартовых состояний.");
            string startStateName = startStates[0].Name;

            return new StatesTable(startStateName, statesTable);
        }

        /// <summary>
        /// Проверка массива символов на повторения символов.
        /// </summary>
        ///<param name="alphabet">Алфавит машины.</param>
        ///<returns>Возвращает результат проверки алфавита на повторяющиеся символы.</returns>

        public static bool CheckAlphabetOnRepeatingChars(char[] alphabet)
        {
            foreach (char c1 in alphabet)
            {
                if (alphabet.Count((c2) => c2 == c1) == 1) continue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Сверка массива символов c алфавитом.
        /// </summary>
        ///<param name="alphabet">Алфавит машины.</param>
        ///<param name="data">
        ///Данные на ленте, которые проверяются на предмет соотношения с алфавитом.
        ///</param>
        ///<param name="charsNotContainedInAlhpabet">Символы, которых нет в алфавите.</param>
        ///<returns>Возвращает результат сверки данных с алфавитом.</returns>

        public static bool ReviseDataWithAlphabet(char[] alphabet, char[] data,
            out List<char> charsNotContainedInAlhpabet)
        {
            charsNotContainedInAlhpabet = new List<char>();
            foreach (char d in data)
            {
                if (alphabet.Contains(d)) continue;
                charsNotContainedInAlhpabet.Add(d);
            }
            return charsNotContainedInAlhpabet.Count == 0;
        }

        /// <summary>
        /// Одиночный запуск машины.
        /// </summary>

        public TMStateStatus Run()
        {
            //TransitionRecord tRecord = States[CurrentStateName].TransitionsColumn[Tape.Data[Head.Offset]];

            char currentChar = Tape.Data[Head.Offset];
            TransitionRecord tRecord = null;
            if (Alphabet.Contains(currentChar))
                tRecord = States[CurrentStateName].TransitionsColumn[currentChar];
            else
                throw new Exception($"Символ {currentChar} отсутствует в словаре.");
            CurrentStateName = States[tRecord.DestTMState].Name;
            Tape.Data[Head.Offset] = tRecord.CharToWrite;
            Head.Offset += (int)tRecord.Move;
            return States[tRecord.DestTMState].Status;
        }

        // Сериализация конфигурации.

        public XElement XSerialize(string tapeDataSource, string tapeDataData)
        {
            XElement xsTMStates = new XElement("TMStates");
            foreach (TMState st in States.Values)
                xsTMStates.Add(st.XSerialize());
            XElement xTM = new XElement("TMConfig",
                new XElement("ProblemFormulation") { Value = ProblemFormulation },
                new XElement("Alphabet") { Value = new string(Alphabet.ToArray()) },
                Tape.XSerialize(tapeDataSource, tapeDataData),
                Head.XSerialize(),
                xsTMStates);
            return xTM;
        }
    }
}
