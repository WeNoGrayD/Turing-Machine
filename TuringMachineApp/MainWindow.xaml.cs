using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using TuringMachineClassLib;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Win32;
using TuringMachineViewModelComponentsLib;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.IO;

namespace TuringMachineApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public TuringMachineViewModel ViewModel { get; set; }

        private TuringMachine _tm;
        public TuringMachine TM
        {
            get { return _tm; }
            set
            {
                if (value != _tm)
                {
                    _tm = value;
                    TuringMachineViewModel.TM = value;
                    OnPropertyChanged(nameof(TM));
                }
            }
        }

        public Label VisualHead;

        private ObservableCollection<int> _tapeIndices;
        public ObservableCollection<int> TapeIndices
        {
            get { return _tapeIndices; }
            set
            {
                if (value != _tapeIndices)
                {
                    _tapeIndices = value;
                    OnPropertyChanged(nameof(TapeIndices));
                }
            }
        }

        private ObservableCollection<char> _tapeData;
        public ObservableCollection<char> TapeData
        {
            get { return _tapeData; }
            set
            {
                if (value != _tapeData)
                {
                    _tapeData = value;
                    OnPropertyChanged(nameof(TapeData));
                }
            }
        }

        public string AdditionCharToAlphabet { get; set; }

        public string AdditionStateName { get; set; }

        public bool SimulationIsRunning { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            XDocument xDoc = new XDocument();

            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            path = path.Remove(path.IndexOf(@"bin\Debug")) + "tm_config.xml";
            xDoc = XDocument.Load(path);

            TM = new TuringMachine(xDoc);
            ViewModel = new TuringMachineViewModel(TM);

            VisualHead = (Label)this.Resources["VisualHead"];
            TapeIndices = new ObservableCollection<int>();
            TapeData = new ObservableCollection<char>();

            InitVisualTape();

            this.DataContext = this;

            unigrdTapeIndices.DataContext = TapeIndices;
            unigrdTape.DataContext = TapeData;

            lstbStates.SelectionChanged += (s, e) =>
            {
                string selectedStateName = lstbStates.SelectedValue as string;
                if (selectedStateName != null)
                    grdSelectedState.DataContext = TM.States[selectedStateName]; 
            };

            SimulationIsRunning = false;
        }

        /// <summary>
        /// Одиночный запуск МТ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SingleRunTuringMachine(object sender, RoutedEventArgs e)
        {
            RunTuringMachine();
        }

        private bool RunTuringMachine()
        {
            if (TM.Head.Offset == -1)
                TM.Tape.Expand(ExpandTo.Left);
            else if (TM.Head.Offset == TM.Tape.Data.Length)
                TM.Tape.Expand(ExpandTo.Right);

            // Поиск следующего движения по ленте окольными дорожками.

            TransitionMove moveTo = TM.States[TM.CurrentStateName]
                                      .TransitionsColumn[TM.Tape.Data[TM.Head.Offset]].Move;
            TM.Run();
            int hi = -1;
            Dispatcher.Invoke(() =>
            {
                do
                {
                    TakeTapeIndices(moveTo);
                    hi = TakeTapeData(moveTo);

                    if (hi > -1)
                    {
                        if (!grdHeadTrack.Children.Contains(VisualHead))
                            grdHeadTrack.Children.Add(VisualHead);
                        Grid.SetColumn(VisualHead, hi);
                        if (hi < 5)
                            moveTo = TransitionMove.ToLeftCell;
                        else if (hi > 5)
                            moveTo = TransitionMove.ToRightCell;
                    }
                    else if (grdHeadTrack.Children.Contains(VisualHead))
                        grdHeadTrack.Children.Remove(VisualHead);
                }
                while (hi != -1 && hi != 5);
            });

            bool isOver = false;
            TMState tmState = TM.States[TM.CurrentStateName];

            if (tmState.Status != TMStateStatus.Usual && tmState.Status != TMStateStatus.Start)
            {
                string mes = "Симуляция машины Тьюринга окончена.";
                mes += " Симуляция завершилась " + 
                    (tmState.Status == TMStateStatus.Accepting ? "допускающим" :
                                                                 "отвергающим") 
                    + $" состоянием {tmState.Name}.";
                mes += $" Комментарий к состоянию: {tmState.Comment}";

                MessageBox.Show(mes);
            }

            return isOver;
        }

        /// <summary>
        /// Запуск МТ в режиме симулации.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SimulateTuringMachine(object sender, RoutedEventArgs e)
        {
            SimulationIsRunning = !SimulationIsRunning;
            if (SimulationIsRunning)
            {
                Task tSimulate = new Task(() =>
                {
                    while (SimulationIsRunning)
                    {
                        bool isOver = RunTuringMachine();
                        Thread.Sleep(200);
                        if (isOver)
                            SimulationIsRunning = false;
                    }
                });
                tSimulate.Start();
            }
        }

        // Инициализация отображения ленты.

        public void InitVisualTape()
        {
            TapeIndices.Clear();
            TapeData.Clear();

            for (int i = 0; i < 11; i++)
            {
                int j = i - 5;
                TapeIndices.Add(j);
                int offset = TM.Head.Offset;
                if (offset + j < 0 || offset + j > TM.Tape.Data.Length)
                    TapeData.Add('_');
                else
                    TapeData.Add(TM.Tape.Data[offset + j]);
            }

            if (!grdHeadTrack.Children.Contains(VisualHead))
                grdHeadTrack.Children.Add(VisualHead);
            Grid.SetColumn(VisualHead, 5);
        }

        // Сдвиг ленты.

        private void ShifTape(object sender, RoutedEventArgs e)
        {
            int moveTo = 0;
            if (((Button)sender).Name.Equals(nameof(btnLeftShift)))
                moveTo = -1;
            else if (((Button)sender).Name.Equals(nameof(btnRightShift)))
                moveTo = 1;
            
            TakeTapeIndices((TransitionMove)moveTo);
            int hi = TakeTapeData((TransitionMove)moveTo);
            if (hi > -1)
            {
                if (!grdHeadTrack.Children.Contains(VisualHead))
                    grdHeadTrack.Children.Add(VisualHead);
                Grid.SetColumn(VisualHead, hi);
            }
            else if (grdHeadTrack.Children.Contains(VisualHead))
                grdHeadTrack.Children.Remove(VisualHead);
        }

        // Инициализация массива индексов ячеек.

        private void TakeTapeIndices(TransitionMove move)
        {
            if (move == TransitionMove.StayDown)
                return;
            
            for (int i = 0; i < TapeIndices.Count; i++)
                TapeIndices[i] += (int)move;
        }

        /// <summary>
        /// Инициализация массива символов в ячейках.
        /// Возвращает индекс ячейки под головкой 
        /// в ОТОБРАЖАЕМОМ массиве.
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>

        private int TakeTapeData(TransitionMove move)
        {
            /*
             * Если движение - "стоять на месте",
             * то необходимо только найти индекс символа под головкой
             * и вписать его в массив символов.
             * Если ячейка с головкой наверху сейчас не видна
             * (лента уехала влево или вправо), то пропускаем.
             * Если движение ленты было влево или вправо,
             * то просто меняем все элементы.
             */

            int hoffset = TM.Head.Offset;
            int vhi = hoffset - TM.Head.DifferenceOffset,
                hi = -1;
            for (int i = 0; i < TapeIndices.Count; i++)
                if (TapeIndices[i] == vhi)
                {
                    hi = i;
                    break;
                }

            if (move == TransitionMove.StayDown && hi > -1)
                TapeData[hi] = TM.Tape.Data[hoffset];
            else if (move != TransitionMove.StayDown)
            {
                int offset = 0;// = vhi + (int)move;

                if (hi > -1)
                {
                    offset = hoffset - hi;// - (int)move;
                }
                else
                {
                    offset = hoffset + (TapeIndices[0] - vhi);// + (int)move;
                    /*
                    if (vhi < TapeIndices[0])
                    {
                        offset = TM.Head.Offset + (TapeIndices[0] - vhi);
                    }
                    else
                    {
                        offset = TM.Head.Offset - (vhi - TapeIndices[0]);
                    }
                    */
                }
                for (int i = 0; i < TapeIndices.Count; i++)
                {
                    if (offset < 0 || offset >= TM.Tape.Data.Length)
                        TapeData[i] = '_';
                    else if (offset < TM.Tape.Data.Length)
                        TapeData[i] = TM.Tape.Data[offset];// + j];
                    offset++;
                }

                //TM.Head.DifferenceOffset -= (int)move;
            }

            return hi;
        }

        // Добавление символа в алфавит.

        private void AddCharToAlphabet(object sender, RoutedEventArgs e)
        {
            if (AdditionCharToAlphabet.Length > 0)
                TM.Alphabet.Add(AdditionCharToAlphabet[0]);
        }

        // Удаление символа из алфавита.

        private void RemoveCharFromAlphabet(object sender, RoutedEventArgs e)
        {
            char deleteChar = (char)lstbAlphabet.SelectedValue;
            if (deleteChar == ' ')
                MessageBox.Show("Символ \"пробел\" ('_') удалить нельзя.");
            else
                TM.Alphabet.Remove((char)lstbAlphabet.SelectedValue);
        }

        // Добавление состояния в таблицу состояний.

        private void AddStateToTable(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(AdditionStateName) && !string.IsNullOrWhiteSpace(AdditionStateName))
                TM.States.Add(AdditionStateName, TMState.CreateEmpty(AdditionStateName, TM));
        }

        // Добавление состояния в таблицу состояний.

        private void RemoveStateFromTable(object sender, RoutedEventArgs e)
        {
            string deleteStateName;
            if ((deleteStateName = lstbStates.SelectedValue as string) != null)
                TM.States.Remove(deleteStateName);
        }

        private void CreateConfig(object sender, RoutedEventArgs e)
        {
            CreateConfigDialogWindow createConfigDW = new CreateConfigDialogWindow();
            if (createConfigDW.ShowDialog() == false)
                return;

            TuringMachine newTM = null;
            try
            {
                newTM = new TuringMachine(createConfigDW.TMConfig);
                List<char> charsNotContainedInAlphabet;
                if (!TuringMachine.ReviseDataWithAlphabet(TM.Alphabet.ToArray(), TM.Tape.Data,
                        out charsNotContainedInAlphabet))
                {
                    StringBuilder mes = new StringBuilder(
                        "В начальных данных встречаются символы,\nотсутствующие в алфавите." +
                        "\nИх перечень:");

                    charsNotContainedInAlphabet.ForEach((ch) => mes.Append(" " + ch.ToString() + ","));
                    mes = mes.Remove(mes.Length - 1, 1);
                    mes.Append('.');
                    throw new Exception(mes.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (newTM != null)
                {
                    TM = newTM;
                    InitVisualTape();
                    lstbStates.SelectedItem = null;
                }
            }
        }

        private void LoadConfig(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openProjDialog = new OpenFileDialog();
            openProjDialog.Filter = "Файлы Extended Markup Language|*.xml";
            openProjDialog.DefaultExt = "xml";
            openProjDialog.Title = "Загрузить конфигурацию...";

            if (openProjDialog.ShowDialog() == false)
                return;

            XDocument xTM = (new XDocument());
            xTM = XDocument.Load(openProjDialog.FileName);

            try
            { this.TM = new TuringMachine(xTM); }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
            finally
            {
                TuringMachineViewModel.TM = TM;

                InitVisualTape();
                lstbStates.SelectedItem = null;
            }
        }

        private void SaveConfig(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openConfigDialog = new OpenFileDialog();
            openConfigDialog.Filter = "Файлы Extended Markup Language|*.xml";
            openConfigDialog.DefaultExt = "xml";
            openConfigDialog.Title = "Сохранить конфигурацию как...";

            if (openConfigDialog.ShowDialog() == false)
                return;

            string tapeDataSource = "", tapeDataData = "";

            if (MessageBox.Show("Желаете ли Вы сохранить данные на ленте?", 
                "Сохранить ленту?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes);
            {
                SaveFileDialog saveConfigDialog = new SaveFileDialog();
                saveConfigDialog.Filter = "Текстовые файлы|*.txt";
                saveConfigDialog.DefaultExt = "txt";
                saveConfigDialog.Title = "Сохранить данные на ленте как...";

                if (saveConfigDialog.ShowDialog() == true)
                    using (StreamWriter swTapeData = new StreamWriter(saveConfigDialog.FileName))
                    {
                        swTapeData.WriteLine(TM.Tape.Data);
                        tapeDataSource = "file";
                        tapeDataData = saveConfigDialog.FileName;
                    }
            }
            if (tapeDataSource == "")
            {
                tapeDataSource = "text";
                tapeDataData = new string(TM.Tape.Data);
            }

            XDocument xTM = new XDocument();

            xTM.Add(TM.XSerialize(tapeDataSource, tapeDataData));

            xTM.Save(openConfigDialog.FileName);
        }

        private void SaveTape(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveConfigDialog = new SaveFileDialog();
            saveConfigDialog.Filter = "Текстовые файлы|*.txt";
            saveConfigDialog.DefaultExt = "txt";
            saveConfigDialog.Title = "Сохранить данные на ленте как...";

            if (saveConfigDialog.ShowDialog() == true)
                using (StreamWriter swTapeData = new StreamWriter(saveConfigDialog.FileName))
                {
                    swTapeData.WriteLine(TM.Tape.Data);
                }
        }

        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            string about = "     Данное приложение является автоматизированной системой моделирования классической машины Тьюринга (МТ)." +
                "\n     Система имеет следущие возможности:" +
                "\n  -- сохранение, загрузка и создание новой конфигурации машины Тьюринга (конфигурация хранится в файле .xml);" +
				"\n  -- сохранение данных на ленте в файл .txt;" +
                "\n  -- широкие возможности редактирования конфигурации МТ с дружелюбным к пользователю интерфейсом, включая" +
                " добавление/удаление символов в алфавите и состояний МТ в таблице состояний;" +
                "\n  -- возможности запуска пошаговой и непрерывной симуляции МТ." +
                "\n     Программа была разработана ст. гр. ПО-18 ИКНТ ДонНТУ Оверчуком И. и Фоминых Н.";

            MessageBox.Show(about);
        }

        private void CloseProgram(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Чтобы вверху таблицы состояний всегда находились их имена,
        /// потребовалось разделить строку с именами и саму таблицу
        /// и выделить их в отдельных скроллвью.
        /// Чтобы строка с именами состояний крутилась синхронно
        /// с таблицей, нужен обработчик события прокрутки 
        /// скроллвью таблицы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void scrvStates_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                scrvStatesNames.ScrollToHorizontalOffset(scrvStatesNames.HorizontalOffset + e.HorizontalChange);
            }
        }

        // Уведомление подписчиков на событие изменения свойства.

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
