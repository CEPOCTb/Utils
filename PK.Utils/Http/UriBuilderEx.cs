using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using JetBrains.Annotations;

namespace PK.Utils.Http
{
	[PublicAPI]
	public class UriBuilderEx
	{
		[NotNull] private readonly UriBuilder _builder;
		[NotNull] private ObservableCollection<string> _segments;
		private bool _segmentsChanged;
		[NotNull] private ObservableDictionary<string, ObservableCollection<string>> _queryParams;
		private bool _queryParamsChanged;

		public int Port
		{
			get => _builder.Port;
			set => _builder.Port = value;
		}

		public string Scheme
		{
			get => _builder.Scheme;
			set => _builder.Scheme = value;
		}

		public string Host
		{
			get => _builder.Host;
			set => _builder.Host = value;
		}

		public string Path
		{
			get
			{
				UpdateSegments();
				return _builder.Path;
			}
			set
			{
				_builder.Path = value;
				ParseSegments();
			}
		}

		public Uri Uri
		{
			get
			{
				UpdateChangedFields();
				return _builder.Uri;
			}
		}

		public string Fragment
		{
			get => _builder.Fragment;
			set => _builder.Fragment = value;
		}

		public string UserName
		{
			get => _builder.UserName;
			set => _builder.UserName = value;
		}

		public string Password
		{
			get => _builder.Password;
			set => _builder.Password = value;
		}

		public string Query
		{
			get
			{
				UpdateQuery();
				return _builder.Query;
			}
			set
			{
				_builder.Query = value;
				ParseQuery();
			}
		}

		public Encoding Encoding { get; set; } = Encoding.UTF8;

		
		public IList<string> Segments => _segments;

		public IDictionary<string, ObservableCollection<string>> Parameters => _queryParams;


		public UriBuilderEx(string uri)
		{
			_builder = new UriBuilder(uri);
			Init();
		}

		public UriBuilderEx(Uri uri)
		{
			_builder = new UriBuilder(uri);
			Init();
		}

		public UriBuilderEx()
		{
			_builder = new UriBuilder();
			Init();
		}

		public UriBuilderEx(string scheme,
			string host,
			int port = 0,
			string path = null,
			string extraValue = null)
		{
			_builder = new UriBuilder(scheme, host, port, path, extraValue);
			Init();
		}

		public void AddParameter(string name, object value)
		{
			AddParameter(name, value, false);
		}

		public void AddParameter(string name, params object[] values)
		{
			if (!_queryParams.TryGetValue(name, out var collection))
			{
				collection = new ObservableCollection<string>();
				_queryParams.Add(name, collection);
			}

			foreach (var value in values)
			{
				collection.Add(value?.ToString());
			}
		}

		public void ReplacePlaceHolders([NotNull] params (string Search, string Replace)[] placeholders)
		{
			for (int i = 0; i < _segments.Count; i++)
			{
				if (_segments[i] != null)
				{
					string newVal = _segments[i]; 
					foreach (var placeholder in placeholders)
					{
						newVal = newVal.Replace(placeholder.Search, placeholder.Replace);
					}

					_segments[i] = newVal;
				}
			}
			
			foreach (var pair in _queryParams)
			{
				if (pair.Value != null)
				{
					for (int i = 0; i < pair.Value.Count; i++)
					{
						string newVal = pair.Value[i];
						foreach (var placeholder in placeholders)
						{
							newVal = newVal.Replace(placeholder.Search, placeholder.Replace);
						}

						pair.Value[i] = newVal;
					}
				}
			}
		}

		#region Overrides of Object

		/// <inheritdoc />
		public override string ToString()
		{
			UpdateChangedFields();
			return _builder.ToString();
		}

		#endregion

		private void Init()
		{
			ParseSegments();
			ParseQuery();
		}

		private void ParseSegments()
		{
			_segments.CollectionChanged -= SegmentsOnCollectionChanged;

			_segments = new ObservableCollection<string>(
				_builder.Path?.Split('/') ?? Enumerable.Empty<string>()
				);
			
			_segments.CollectionChanged += SegmentsOnCollectionChanged;

			_segmentsChanged = true;
		}

		private void AddParameter(string name, object value, bool registerHandlers)
		{
			if (!_queryParams.TryGetValue(name, out var collection))
			{
				collection = new ObservableCollection<string>();
				if (registerHandlers)
				{
					collection.CollectionChanged += ParametersValueOnCollectionChanged;
				}
				_queryParams.Add(name, collection);
			}

			collection.Add(value?.ToString());
		}

		private void ParseQuery()
		{
			_queryParams.CollectionChanged -= ParametersOnCollectionChanged;

			foreach (var pair in _queryParams)
			{
				pair.Value.CollectionChanged -= ParametersValueOnCollectionChanged;
			}

			_queryParams = new ObservableDictionary<string, ObservableCollection<string>>();
			_queryParams.CollectionChanged += ParametersOnCollectionChanged;

			var query = _builder.Query;
			int start = -1;
			string name = null;

			var lastIndex = query.Length - 1;
			
			for (int i = 0; i < query.Length; i++)
			{
				if (query[i] == '?' && i == 0)
				{
					continue;
				}

				if (start < 0)
				{
					start = i;
				}

				if (query[i] == '&' || i == lastIndex)
				{
					if (name == null && start != i) // no value
					{
						AddParameter(HttpUtility.UrlDecode(query[start..i], Encoding), null, true);
					}
					else if (name != null)
					{
						AddParameter(name, HttpUtility.UrlDecode(query[start..i], Encoding), true);
					}

					name = null;
					start = -1;
					continue;
				}
				
				if (query[i] == '=')
				{
					name = HttpUtility.UrlDecode(query[start..i], Encoding);
				}
			}
			_queryParamsChanged = true;
		}

		private void SegmentsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			_segmentsChanged = true;
		}
		
		private void ParametersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			_queryParamsChanged = true;
			if (e.OldItems != null)
			{
				foreach (var oldItem in e.OldItems)
				{
					var collection = (KeyValuePair<string, ObservableCollection<string>>) oldItem;
					collection.Value.CollectionChanged -= ParametersValueOnCollectionChanged;
				}
			}

			if (e.NewItems != null)
			{
				foreach (var newItem in e.NewItems)
				{
					var collection = (KeyValuePair<string, ObservableCollection<string>>) newItem;
					collection.Value.CollectionChanged -= ParametersValueOnCollectionChanged;
					collection.Value.CollectionChanged += ParametersValueOnCollectionChanged;
				}
			}
		}

		private void ParametersValueOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			_queryParamsChanged = true;
		}

		private void UpdateChangedFields()
		{
			UpdateSegments();
			UpdateQuery();
		}
		
		private void UpdateSegments()
		{
			if (_segmentsChanged)
			{
				_builder.Path = string.Join('/', _segments.Select(s => s.Trim()));
				_segmentsChanged = false;
			}
		}

		private void UpdateQuery()
		{
			if (_queryParamsChanged)
			{
				if (_queryParams.Count <= 0)
				{
					_builder.Query = string.Empty;
					return;
				}

				var builder = new StringBuilder();
				bool first = true;

				foreach (var pair in _queryParams)
				{
					if (pair.Value == null)
					{
						if (first)
						{
							first = false;
						}
						else
						{
							builder.Append('&');
						}

						builder.Append(HttpUtility.UrlEncode(pair.Key, Encoding));
					}
					else
					{
						foreach (var value in pair.Value)
						{
							if (first)
							{
								first = false;
							}
							else
							{
								builder.Append('&');
							}

							builder.Append(HttpUtility.UrlEncode(pair.Key, Encoding));
							if (pair.Value != null)
							{
								builder.Append('=');
								builder.Append(HttpUtility.UrlEncode(value, Encoding));
							}
						}
					}
				}

				_builder.Query = builder.ToString();

				_queryParamsChanged = false;
			}
		}
	}
}
