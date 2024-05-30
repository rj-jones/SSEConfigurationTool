using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;

namespace SSEConfigurationTool.src
{
    internal class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                if (base.ContainsKey(key))
                {
                    var oldValue = base[key];
                    base[key] = value;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        new KeyValuePair<TKey, TValue>(key, value),
                        new KeyValuePair<TKey, TValue>(key, oldValue)));
                }
                else
                {
                    base[key] = value;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                        new KeyValuePair<TKey, TValue>(key, value)));
                }
                OnPropertyChanged("Item[]");
            }
        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                new KeyValuePair<TKey, TValue>(key, value)));
            OnPropertyChanged("Item[]");
        }

        public new bool Remove(TKey key)
        {
            if (base.ContainsKey(key))
            {
                var value = base[key];
                var result = base.Remove(key);
                if (result)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                        new KeyValuePair<TKey, TValue>(key, value)));
                    OnPropertyChanged("Item[]");
                    return true;
                }
            }
            return false;
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
