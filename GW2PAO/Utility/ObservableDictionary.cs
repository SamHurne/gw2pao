using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace GW2PAO.Utility
{
    [Serializable]
    public class ObservableDictionary<TKey, TVal> :
        IDictionary<TKey, TVal>,
        ICollection,
        IXmlSerializable,
        ISerializable,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        private const string ItemNodeName = "Item";
        private const string KeyNodeName = "Key";
        private const string ValueNodeName = "Value";

        protected KeyedDictionaryEntryCollection<TKey> keyedEntryCollection;
        private int countCache = 0;
        private Dictionary<TKey, TVal> dictionaryCache = new Dictionary<TKey, TVal>();
        private int dictionaryCacheVersion = 0;
        private int version = 0;
        private XmlSerializer keySerializer = null;
        private XmlSerializer valueSerializer = null;


        public ObservableDictionary()
        {
            this.keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();
        }

        public ObservableDictionary(IDictionary<TKey, TVal> dictionary)
        {
            this.keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();

            foreach (KeyValuePair<TKey, TVal> entry in dictionary)
                this.DoAddEntry((TKey)entry.Key, (TVal)entry.Value);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            this.keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);
        }

        public ObservableDictionary(IDictionary<TKey, TVal> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);

            foreach (KeyValuePair<TKey, TVal> entry in dictionary)
                this.DoAddEntry((TKey)entry.Key, (TVal)entry.Value);
        }

        protected ObservableDictionary(SerializationInfo info, StreamingContext context)
        {
            int itemCount = info.GetInt32("ItemCount");
            for (int i = 0; i < itemCount; i++)
            {
                KeyValuePair<TKey, TVal> kvp = (KeyValuePair<TKey, TVal>)info.GetValue(String.Format("Item{0}", i), typeof(KeyValuePair<TKey, TVal>));
                this.Add(kvp.Key, kvp.Value);
            }
        }


        public IEqualityComparer<TKey> Comparer
        {
            get { return this.keyedEntryCollection.Comparer; }
        }

        public int Count
        {
            get { return this.keyedEntryCollection.Count; }
        }

        public Dictionary<TKey, TVal>.KeyCollection Keys
        {
            get { return this.TrueDictionary.Keys; }
        }

        ICollection<TKey> IDictionary<TKey, TVal>.Keys
        {
            get
            {
                return Keys;
            }
        }

        public TVal this[TKey key]
        {
            get { return (TVal)this.keyedEntryCollection[key].Value; }
            set { this.DoSetEntry(key, value); }
        }

        public Dictionary<TKey, TVal>.ValueCollection Values
        {
            get { return this.TrueDictionary.Values; }
        }

        ICollection<TVal> IDictionary<TKey, TVal>.Values
        {
            get
            {
                return Values;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        private Dictionary<TKey, TVal> TrueDictionary
        {
            get
            {
                if (this.dictionaryCacheVersion != version)
                {
                    this.dictionaryCache.Clear();
                    foreach (DictionaryEntry entry in keyedEntryCollection)
                        this.dictionaryCache.Add((TKey)entry.Key, (TVal)entry.Value);
                    this.dictionaryCacheVersion = version;
                }
                return this.dictionaryCache;
            }
        }

        public void Add(TKey key, TVal value)
        {
            this.DoAddEntry(key, value);
        }

        public void Add(KeyValuePair<TKey, TVal> item)
        {
            this.DoAddEntry(item.Key, item.Value);
        }

        public void Clear()
        {
            this.DoClearEntries();
        }

        public bool Contains(KeyValuePair<TKey, TVal> item)
        {
            return this.keyedEntryCollection.Contains(item.Key);
        }

        public bool ContainsKey(TKey key)
        {
            return this.keyedEntryCollection.Contains(key);
        }

        public bool ContainsValue(TVal value)
        {
            return this.TrueDictionary.ContainsValue(value);
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator<TKey, TVal>(this, false);
        }

        public bool Remove(TKey key)
        {
            return this.DoRemoveEntry(key);
        }

        public bool Remove(KeyValuePair<TKey, TVal> item)
        {
            return this.DoRemoveEntry(item.Key);
        }

        public bool TryGetValue(TKey key, out TVal value)
        {
            bool result = this.keyedEntryCollection.Contains(key);
            value = result ? (TVal)this.keyedEntryCollection[key].Value : default(TVal);
            return result;
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)this.keyedEntryCollection).SyncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)this.keyedEntryCollection).IsSynchronized;
            }
        }

        IEnumerator<KeyValuePair<TKey, TVal>> IEnumerable<KeyValuePair<TKey, TVal>>.GetEnumerator()
        {
            return new Enumerator<TKey, TVal>(this, false);
        }

        public void CopyTo(KeyValuePair<TKey, TVal>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("CopyTo() failed:  array parameter was null");
            }
            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                throw new ArgumentOutOfRangeException("CopyTo() failed:  index parameter was outside the bounds of the supplied array");
            }
            if ((array.Length - arrayIndex) < this.keyedEntryCollection.Count)
            {
                throw new ArgumentException("CopyTo() failed:  supplied array was too small");
            }

            foreach (DictionaryEntry entry in this.keyedEntryCollection)
                array[arrayIndex++] = new KeyValuePair<TKey, TVal>((TKey)entry.Key, (TVal)entry.Value);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)this.keyedEntryCollection).CopyTo(array, index);
        }

        protected virtual bool AddEntry(TKey key, TVal value)
        {
            this.keyedEntryCollection.Add(new DictionaryEntry(key, value));
            return true;
        }

        protected virtual bool ClearEntries()
        {
            bool entriesToClear = (Count > 0);
            if (entriesToClear)
            {
                this.keyedEntryCollection.Clear();
            }
            return entriesToClear;
        }

        protected int GetIndexAndEntryForKey(TKey key, out DictionaryEntry entry)
        {
            entry = new DictionaryEntry();
            int index = -1;
            if (this.keyedEntryCollection.Contains(key))
            {
                entry = this.keyedEntryCollection[key];
                index = this.keyedEntryCollection.IndexOf(entry);
            }
            return index;
        }

        protected virtual bool RemoveEntry(TKey key)
        {
            return this.keyedEntryCollection.Remove(key);
        }

        protected virtual bool SetEntry(TKey key, TVal value)
        {
            bool keyExists = this.keyedEntryCollection.Contains(key);

            // if identical key/value pair already exists, nothing to do
            if (keyExists && value.Equals((TVal)this.keyedEntryCollection[key].Value))
                return false;

            // otherwise, remove the existing entry
            if (keyExists)
                this.keyedEntryCollection.Remove(key);

            // add the new entry
            this.keyedEntryCollection.Add(new DictionaryEntry(key, value));

            return true;
        }

        private void DoAddEntry(TKey key, TVal value)
        {
            if (this.AddEntry(key, value))
            {
                version++;

                DictionaryEntry entry;
                int index = this.GetIndexAndEntryForKey(key, out entry);
                this.FireEntryAddedNotifications(entry, index);
            }
        }

        private void DoClearEntries()
        {
            if (this.ClearEntries())
            {
                version++;
                this.FireResetNotifications();
            }
        }

        private bool DoRemoveEntry(TKey key)
        {
            DictionaryEntry entry;
            int index = this.GetIndexAndEntryForKey(key, out entry);

            bool result = this.RemoveEntry(key);
            if (result)
            {
                version++;
                if (index > -1)
                    this.FireEntryRemovedNotifications(entry, index);
            }

            return result;
        }

        private void DoSetEntry(TKey key, TVal value)
        {
            DictionaryEntry entry;
            int index = this.GetIndexAndEntryForKey(key, out entry);

            if (this.SetEntry(key, value))
            {
                version++;

                // if prior entry existed for this key, fire the removed notifications
                if (index > -1)
                {
                    this.FireEntryRemovedNotifications(entry, index);

                    // force the property change notifications to fire for the modified entry
                    countCache--;
                }

                // then fire the added notifications
                index = this.GetIndexAndEntryForKey(key, out entry);
                this.FireEntryAddedNotifications(entry, index);
            }
        }

        private void FireEntryAddedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            this.FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TVal>((TKey)entry.Key, (TVal)entry.Value), index));
            else
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void FireEntryRemovedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            this.FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TVal>((TKey)entry.Key, (TVal)entry.Value), index));
            else
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void FirePropertyChangedNotifications()
        {
            if (this.Count != countCache)
            {
                countCache = Count;
                this.OnPropertyChanged("Count");
                this.OnPropertyChanged("Item[]");
                this.OnPropertyChanged("Keys");
                this.OnPropertyChanged("Values");
            }
        }

        private void FireResetNotifications()
        {
            // fire the relevant PropertyChanged notifications
            this.FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #region ISerializable

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemCount", this.Count);
            int itemIdx = 0;
            foreach (KeyValuePair<TKey, TVal> kvp in this)
            {
                info.AddValue(String.Format("Item{0}", itemIdx), kvp, typeof(KeyValuePair<TKey, TVal>));
                itemIdx++;
            }
        }

        #endregion

        #region IXmlSerializable

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (KeyValuePair<TKey, TVal> kvp in this)
            {
                writer.WriteStartElement(ItemNodeName);
                writer.WriteStartElement(KeyNodeName);
                KeySerializer.Serialize(writer, kvp.Key);
                writer.WriteEndElement();
                writer.WriteStartElement(ValueNodeName);
                ValueSerializer.Serialize(writer, kvp.Value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            // Move past container
            if (!reader.Read())
            {
                throw new XmlException("Error in Deserialization of Dictionary");
            }

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement(ItemNodeName);
                reader.ReadStartElement(KeyNodeName);
                TKey key = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement(ValueNodeName);
                TVal value = (TVal)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadEndElement();
                this.Add(key, value);
                reader.MoveToContent();
            }

            // Read End Element to close Read of containing node
            reader.ReadEndElement();
        }

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        protected XmlSerializer ValueSerializer
        {
            get
            {
                if (valueSerializer == null)
                {
                    valueSerializer = new XmlSerializer(typeof(TVal));
                }
                return valueSerializer;
            }
        }

        private XmlSerializer KeySerializer
        {
            get
            {
                if (keySerializer == null)
                {
                    keySerializer = new XmlSerializer(typeof(TKey));
                }
                return keySerializer;
            }
        }

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, args);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        protected class KeyedDictionaryEntryCollection<Tk> : KeyedCollection<Tk, DictionaryEntry>
        {
            public KeyedDictionaryEntryCollection() : base() { }

            public KeyedDictionaryEntryCollection(IEqualityComparer<Tk> comparer) : base(comparer) { }

            protected override Tk GetKeyForItem(DictionaryEntry entry)
            {
                return (Tk)entry.Key;
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator<Tk, TValue> : IEnumerator<KeyValuePair<Tk, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
        {
            private ObservableDictionary<Tk, TValue> _dictionary;
            private int _version;
            private int _index;
            private KeyValuePair<Tk, TValue> _current;
            private bool _isDictionaryEntryEnumerator;

            internal Enumerator(ObservableDictionary<Tk, TValue> dictionary, bool isDictionaryEntryEnumerator)
            {
                _dictionary = dictionary;
                _version = dictionary.version;
                _index = -1;
                _isDictionaryEntryEnumerator = isDictionaryEntryEnumerator;
                _current = new KeyValuePair<Tk, TValue>();
            }

            public KeyValuePair<Tk, TValue> Current
            {
                get
                {
                    ValidateCurrent();
                    return _current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ValidateVersion();
                _index++;
                if (_index < _dictionary.keyedEntryCollection.Count)
                {
                    _current = new KeyValuePair<Tk, TValue>((Tk)_dictionary.keyedEntryCollection[_index].Key, (TValue)_dictionary.keyedEntryCollection[_index].Value);
                    return true;
                }
                _index = -2;
                _current = new KeyValuePair<Tk, TValue>();
                return false;
            }

            private void ValidateCurrent()
            {
                if (_index == -1)
                {
                    throw new InvalidOperationException("The enumerator has not been started.");
                }
                else if (_index == -2)
                {
                    throw new InvalidOperationException("The enumerator has reached the end of the collection.");
                }
            }

            private void ValidateVersion()
            {
                if (_version != _dictionary.version)
                {
                    throw new InvalidOperationException("The enumerator is not valid because the dictionary changed.");
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    ValidateCurrent();
                    if (_isDictionaryEntryEnumerator)
                    {
                        return new DictionaryEntry(_current.Key, _current.Value);
                    }
                    return new KeyValuePair<Tk, TValue>(_current.Key, _current.Value);
                }
            }

            void IEnumerator.Reset()
            {
                ValidateVersion();
                _index = -1;
                _current = new KeyValuePair<Tk, TValue>();
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    ValidateCurrent();
                    return new DictionaryEntry(_current.Key, _current.Value);
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    ValidateCurrent();
                    return _current.Key;
                }
            }

            object IDictionaryEnumerator.Value
            {
                get
                {
                    ValidateCurrent();
                    return _current.Value;
                }
            }
        }
    }
}
