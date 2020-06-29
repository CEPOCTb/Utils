using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PK.Utils.Observable
{
	/// <summary>
	/// Dictionary with change notification
	/// </summary>
	/// <typeparam name="TKey">Key type</typeparam>
	/// <typeparam name="TValue">Value type</typeparam>
	[PublicAPI]
	public class ObservableDictionary<TKey, TValue>
		: IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		[NotNull]
		private readonly IDictionary<TKey, TValue> _dictionary;

		/// <summary>
		/// Constructor
		/// </summary>
		public ObservableDictionary() => _dictionary = new Dictionary<TKey, TValue>();

		/// <summary>
		/// Copy-constructor
		/// </summary>
		/// <param name="dictionary">Source dictionary</param>
		public ObservableDictionary([NotNull] IDictionary<TKey, TValue> dictionary) =>
			_dictionary = new Dictionary<TKey, TValue>(dictionary);

		/// <summary>
		/// Copy-constructor
		/// </summary>
		/// <param name="dictionary">Source dictionary</param>
		/// <param name="comparer">Key equality comparer</param>
		public ObservableDictionary(
			[NotNull] IDictionary<TKey, TValue> dictionary,
			[CanBeNull] IEqualityComparer<TKey> comparer
			) =>
			_dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="comparer">Key equality comparer</param>
		public ObservableDictionary([CanBeNull] IEqualityComparer<TKey> comparer) =>
			_dictionary = new Dictionary<TKey, TValue>(comparer);

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="capacity">Initial capacity</param>
		public ObservableDictionary(int capacity) => _dictionary = new Dictionary<TKey, TValue>(capacity);

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="capacity">Initial capacity</param>
		/// <param name="comparer">Key equality comparer</param>
		public ObservableDictionary(int capacity, [CanBeNull] IEqualityComparer<TKey> comparer) =>
			_dictionary = new Dictionary<TKey, TValue>(capacity, comparer);

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_dictionary).GetEnumerator();

		/// <inheritdoc />
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			_dictionary.Add(item);
			OnCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add,
					item)
				);
		}

		/// <inheritdoc />
		public void Clear()
		{
			_dictionary.Clear();
			OnCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset)
				);
		}

		/// <inheritdoc />
		public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

		/// <inheritdoc />
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);

		/// <inheritdoc />
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			var result = _dictionary.Remove(item);
			if (result)
			{
				OnCollectionChanged(
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Remove,
						item)
					);
			}

			return result;
		}

		/// <inheritdoc />
		public int Count => _dictionary.Count;

		/// <inheritdoc />
		public bool IsReadOnly => _dictionary.IsReadOnly;

		/// <inheritdoc />
		public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

		/// <inheritdoc />
		public void Add(TKey key, TValue value)
		{
			_dictionary.Add(key, value);
			OnCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add,
					new KeyValuePair<TKey, TValue>(key, value)
					)
				);
		}

		/// <inheritdoc />
		public bool Remove(TKey key)
		{
			if (!_dictionary.TryGetValue(key, out var val) || !_dictionary.Remove(key))
			{
				return false;
			}

			OnCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Remove,
					new KeyValuePair<TKey, TValue>(key, val)
					)
				);
			return true;
		}

		/// <inheritdoc />
		public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

		/// <inheritdoc />
		public TValue this[TKey key]
		{
			get => _dictionary[key];
			set
			{
				var eventArgs = _dictionary.TryGetValue(key, out var oldVal)
					? new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Replace,
						new KeyValuePair<TKey, TValue>(key, value),
						new KeyValuePair<TKey, TValue>(key, oldVal)
						)
					: new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Add,
						new KeyValuePair<TKey, TValue>(key, value)
						);

				_dictionary[key] = value;

				OnCollectionChanged(eventArgs);
			}
		}

		/// <inheritdoc />
		public ICollection<TKey> Keys => _dictionary.Keys;

		/// <inheritdoc />
		public ICollection<TValue> Values => _dictionary.Values;

		/// <inheritdoc />
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Notifies observers of changing of collection content
		/// </summary>
		/// <param name="eventArgs">Arguments describing change</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
		{
			CollectionChanged?.Invoke(this, eventArgs);

			switch (eventArgs.Action)
			{
				case NotifyCollectionChangedAction.Add:
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Reset:
					OnPropertyChanged(nameof(Count));
					OnPropertyChanged(nameof(Keys));
					break;
			}

			OnPropertyChanged(nameof(Values));
		}

		/// <summary>
		/// Notifies observers of changing a property value
		/// </summary>
		/// <param name="propertyName">Changed property name</param>
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
