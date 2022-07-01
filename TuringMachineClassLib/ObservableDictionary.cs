using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TuringMachineClassLib
{
    // Мне нужен тип, состоящий из двух изменяемых переменных.
    // Такого нет.
    // Поэтому вот мой.

    public class KeyValueTuple<TKey, TValue>
    {
        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public KeyValueTuple(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public class ObservableDictionary<TKey, TValue> : ObservableCollection<KeyValueTuple<TKey, TValue>>,
                                                      INotifyPropertyChanged
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
        // Внутренний словарь.

        private Dictionary<TKey, TValue> _content;

        // Индексатор, дающий доступ к значениям словаря по ключу.

        public TValue this[TKey key]
        {
            get { return _content[key]; }
            set
            {
                if (!_content[key].Equals(value))
                {
                    _content[key] = value;
                    this.First(kvp => kvp.Key.Equals(key)).Value = value;
                    OnPropertyChanged(key);
                }
            }
        }

        // Простой доступ к ключам словаря.

        private ObservableCollection<TKey> _keys;
        public ObservableCollection<TKey> Keys
        {
            get { return _keys; }
            set
            {
                if (value != _keys)
                {
                    _keys = value;
                    OnPropertyChanged(nameof(Keys));
                }
            }
        }

        // Простой доступ к значениям словаря.

        private ObservableCollection<TValue> _values;
        public ObservableCollection<TValue> Values
        {
            get { return _values; }
            set
            {
                if (value != _values)
                {
                    _values = value;
                    OnPropertyChanged(nameof(Values));
                }
            }
        }

        // Конструктор.

        public ObservableDictionary() : base()
        {
            _content = new Dictionary<TKey, TValue>();
            this.Keys = new ObservableCollection<TKey>();
            this.Values = new ObservableCollection<TValue>();
            this.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        {
                            _content.Clear();
                            this.Keys.Clear();
                            this.Values.Clear();
                            break;
                        }
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        {
                            foreach (KeyValueTuple<TKey, TValue> newItem in
                                     e.NewItems.Cast<KeyValueTuple<TKey, TValue>>())
                            {
                                _content.Add(newItem.Key, newItem.Value);
                                this.Keys.Add(newItem.Key);
                                this.Values.Add(newItem.Value);
                            }
                            break;
                        }
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        {
                            foreach (KeyValueTuple<TKey, TValue> oldItem in
                                     e.OldItems.Cast<KeyValueTuple<TKey, TValue>>())
                            {
                                _content.Remove(oldItem.Key);
                                this.Keys.Remove(oldItem.Key);
                                this.Values.Remove(oldItem.Value);
                            }
                            break;
                        }
                }
            };
        }

        public ObservableDictionary(Dictionary<TKey, TValue> dict) : this()
        {
            foreach (TKey key in dict.Keys)
                this.Add(key, dict[key]);
        }

        // Перегрузка метода Add.

        public void Add(TKey key, TValue value)
        {
            this.Add(new KeyValueTuple<TKey, TValue>(key, value));
        }

        // Перегрузка метода AddRange.

        public void AddRange(IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            foreach(var kvp in keys.Zip(values, (key, value) => new KeyValueTuple<TKey, TValue>(key, value)))
                this.Add(kvp);
        }

        // Перегрузка метода Remove.

        public void Remove (TKey key)
        {
            if (!_content.ContainsKey(key))
                return;

            this.Remove(this.First(i => i.Key.Equals(key)));
        }

        // Попытка получения значения из словаря.

        public bool TryGetValue (TKey key, out TValue value)
        {
            return _content.TryGetValue(key, out value);
        }

        // Поиск ключа в словаре.

        public bool ContainsKey(TKey key)
        {
            return _content.ContainsKey(key);
        }

        // Уведомление подписчиков на событие изменения свойства.

        public new event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop.ToString()));
        }

        private void OnPropertyChanged(TKey prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop.ToString()));
        }
    }
}
