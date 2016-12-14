namespace PasswordStorageLib
{
    using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.Management.Automation;
	using System.Security;
    using System.Windows.Forms;
    using System.Xml;
    public class WindowOwner : IWin32Window
    {
        private IntPtr _handle;
        public IntPtr Handle { get { return _handle; } }
        public WindowOwner(IntPtr handle) { _handle = handle; }
		public WindowOwner() : this(GetCurrentProcessWindowHandle()) { }
		public static IntPtr GetCurrentProcessWindowHandle()
		{
			using (Process process = Process.GetCurrentProcess())
				return process.MainWindowHandle;
		}
    }
	public static class StringHelper
	{
		public static bool IsNullOrWhiteSpace(string value) { return value == null || value.TrimEnd().Length == 0; }
		public static string DefaultIfNull(string value, string defaultValue) { return (value == null) ? defaultValue : value; }
		public static string DefaultIfNullOrEmpty(string value, string defaultValue) { return (String.IsNullOrEmpty(value)) ? defaultValue : value; }
		public static string DefaultIfNullOrWhiteSpace(string value, string defaultValue) { return (IsNullOrWhiteSpace(value)) ? defaultValue : value; }
	}
	public delegate TResult SelectValueHandler<TSource, TResult>(TSource value);
	public delegate TResult SelectValueStateHandler<TState, TSource, TResult>(TState state, TSource value);
	public delegate TResult AggregateHandler<TSource, TResult>(TResult accumulate, TSource value);
	public delegate TResult AggregateStateHandler<TState, TSource, TResult>(TState state, TResult accumulate, TSource value);
	public delegate bool StatePredicate<TState, TSource>(TState state, TSource value);
	public static class EnumerationHelper
	{
		public static IList<T> NullToEmpty<T>(IList<T> values)
		{
			if (values == null)
				return new T[0];
			return values;
		}
		public static IList<T> AsList<T>(IEnumerable<T> values)
		{
			if (values == null)
				return new T[0];
			if (values is IList<T>)
				return values as IList<T>;
			return new List<T>(values);
		}
		public static TResult Aggregate<TSource, TResult>(IEnumerable<TSource> source, TResult seed, AggregateHandler<TSource, TResult> handler)
		{
			if (source == null || handler == null)
				return seed;
			TResult accumulate = seed;
			foreach (TSource value in source)
				accumulate = handler(accumulate, value);
            return accumulate;
		}
		public static TResult Aggregate<TState, TSource, TResult>(TState state, IEnumerable<TSource> source, TResult seed, AggregateStateHandler<TState, TSource, TResult> handler)
		{
			if (source == null || handler == null)
				return seed;
			TResult accumulate = seed;
			foreach (TSource value in source)
				accumulate = handler(state, accumulate, value);
            return accumulate;
		}
		public static IEnumerable<T> OfType<T>(IEnumerable source)
		{
			if (source != null)
			{
				foreach (object obj in source)
				{
					if (obj != null && obj is T)
						yield return (T)obj;
				}
			}
		}
		public static IEnumerable<TResult> Select<TSource, TResult>(IEnumerable<TSource> values, SelectValueHandler<TSource, TResult> handler)
		{
			if (values != null && handler != null)
			{
				foreach (TSource v in values)
					yield return handler(v);
			}
		}
		public static IEnumerable<TResult> Select<TState, TSource, TResult>(TState state, IEnumerable<TSource> values, SelectValueStateHandler<TState, TSource, TResult> handler)
		{
			if (values != null && handler != null)
			{
				foreach (TSource v in values)
					yield return handler(state, v);
			}
		}
		public static IEnumerable<T> Where<T>(IEnumerable<T> values, Predicate<T> predicate)
		{
			if (values == null || predicate == null)
				yield break;
			
			foreach (T v in values)
			{
				if (predicate(v))
					yield return v;
			}
		}
		public static IEnumerable<TSource> Where<TState, TSource>(TState state, IEnumerable<TSource> values, StatePredicate<TState, TSource> predicate)
		{
			if (values == null || predicate == null)
				yield break;
			
			foreach (TSource v in values)
			{
				if (predicate(state, v))
					yield return v;
			}
		}
		public static bool StructEqualsPredicateHandler<T>(T x, T y) where T : struct, IComparable { return x.CompareTo(y) == 0; }
		public static IEnumerable<T> SkipWhile<T>(IEnumerable<T> values, Predicate<T> predicate)
		{
			if (values == null || predicate == null)
				yield break;
			
			using (IEnumerator<T> enumerator = values.GetEnumerator())
			{
				do
				{
					if (!enumerator.MoveNext())
					{
						predicate = null;
						break;
					}
				} while (predicate(enumerator.Current));
				if (predicate != null)
				{
					yield return enumerator.Current;
					while (enumerator.MoveNext())
						yield return enumerator.Current;
				}
			}
		}
		public static IEnumerable<TSource> SkipWhile<TState, TSource>(TState state, IEnumerable<TSource> values, StatePredicate<TState, TSource> predicate)
		{
			if (values == null || predicate == null)
				yield break;
			
			using (IEnumerator<TSource> enumerator = values.GetEnumerator())
			{
				do
				{
					if (!enumerator.MoveNext())
					{
						predicate = null;
						break;
					}
				} while (predicate(state, enumerator.Current));
				if (predicate != null)
				{
					yield return enumerator.Current;
					while (enumerator.MoveNext())
						yield return enumerator.Current;
				}
			}
		}
		public static IEnumerable<T> Skip<T>(IEnumerable<T> values, int count)
		{
			if (values == null || count < 1)
				yield break;
			
			using (IEnumerator<T> enumerator = values.GetEnumerator())
			{
				while (count > 0)
				{
					if (!enumerator.MoveNext())
						break;
				}
				if (count == 0)
				{
					while (enumerator.MoveNext())
						yield return enumerator.Current;
				}
			}
		}
		public static bool Any<T>(IEnumerable<T> values)
		{
			if (values == null)
				return false;
			
			bool result;
			using (IEnumerator<T> enumerator = values.GetEnumerator())
				result = enumerator.MoveNext();
				
			return result;
		}
		public static bool Any<T>(IEnumerable<T> values, Predicate<T> predicate)
		{
			if (values == null || predicate == null)
				return false;
			
			foreach (T v in values)
			{
				if (predicate(v))
					return true;
			}
			
			return false;
		}
		public static bool Any<TState, TSource>(TState state, IEnumerable<TSource> values, StatePredicate<TState, TSource> predicate)
		{
			if (values == null || predicate == null)
				return false;
			
			foreach (TSource v in values)
			{
				if (predicate(state, v))
					return true;
			}
			
			return false;
		}
		public static IEnumerable<T> ReplaceLast<T>(IEnumerable<T> values, T replacement)
		{
			if (values != null)
			{
				using (IEnumerator<T> enumerator = values.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						T current = enumerator.Current;
						while (enumerator.MoveNext())
						{
							yield return current;
							current = enumerator.Current;
						}
					}
				}
			}
			yield return replacement;
		}
		public static IEnumerable<T> DefaultIfEmpty<T>(IEnumerable<T> values, T defaultValue)
		{
			if (values != null)
			{
				using (IEnumerator<T> enumerator = values.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						do { yield return enumerator.Current; } while (enumerator.MoveNext());
						yield break;
					}
				}
			}
			
			yield return defaultValue;
		}
		public static bool NotNullPredicateHandler<T>(T value) where T : class { return value != null; }
		public static IEnumerable<T> WhereNotNull<T>(IEnumerable<T> values) where T : class { return Where<T>(values, new Predicate<T>(NotNullPredicateHandler<T>)); }
	}
	public abstract class PasswordStorageFormBase : Form
	{
		#region Fields
		private IContainer _components = new Container();
		private Collection<PSObject> _innerErrors = new Collection<PSObject>();
		private ReadOnlyCollection<PSObject> _errors;
		#endregion
		#region Properties
		public ReadOnlyCollection<PSObject> Errors { get { return _errors; } }
		protected IContainer Components { get { return _components; } }
		#endregion
		#region Constructors / Initialization
		protected PasswordStorageFormBase(string windowTitle, Size size, string name)
		{
			_errors = new ReadOnlyCollection<PSObject>(_innerErrors);
			SuspendLayout();
			try
    		{
				TopLevel = true;
    			ClientSize = size;
    			if (windowTitle == null || windowTitle.Trim().Length == 0)
    			{
    				Text = GetType().Name;
    				Name = XmlConvert.EncodeLocalName((StringHelper.IsNullOrWhiteSpace(name)) ? String.Format("{0}{1}", Text, Guid.NewGuid().ToString("N")) : name);
    			}
    			else
    			{
    				Text = windowTitle;
    				Name = XmlConvert.EncodeLocalName((StringHelper.IsNullOrWhiteSpace(name)) ? String.Format("{0}{1}", GetType().Name, Guid.NewGuid().ToString("N")) : name);
    			}
                OnFormInitializing();
            }
			finally
			{
				ResumeLayout(false);
				OnFormInitialized();
				PerformLayout();
			}
		}
		protected PasswordStorageFormBase(string windowTitle, string name) : this(windowTitle, new Size(800, 600), name) { }
		protected virtual void OnFormInitializing() { }
		protected virtual void OnFormInitialized() { }
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				IContainer components = _components;
				_components = null;
				if (components != null)
					components.Dispose();
			}
			
			base.Dispose(disposing);
		}
		#endregion
		#region Errors
		protected void AddError(object obj)
		{
			if (obj != null)
				_innerErrors.Add((obj is PSObject) ? obj as PSObject : PSObject.AsPSObject(obj));
		}
		public void ClearErrors() { _innerErrors.Clear(); }
		#endregion
		#region InitializeTableLayoutPanel
		private static float GetAutoPercentage(IEnumerable<bool> values)
		{
			if (values == null)
				return 100.0f;
			
			float result = 100.0f;
			float next = 100.0f;
			foreach (bool b in values)
			{
				if (b)
				{
					result = next;
					next = next / 2.0f;
				}
			}
			return result;
		}
		protected static ColumnStyle BoolToColumnStyle(float percentage, bool isAutoSize) { return (isAutoSize) ? new ColumnStyle(SizeType.AutoSize) : new ColumnStyle(SizeType.Percent, percentage); }
		protected static RowStyle BoolToRowStyle(float percentage, bool isAutoSize) { return (isAutoSize) ? new RowStyle(SizeType.AutoSize) : new RowStyle(SizeType.Percent, percentage); }
		protected static void InitializeTableLayoutPanel(TableLayoutPanel panel, IEnumerable<ColumnStyle> columns, IEnumerable<RowStyle> rows)
		{
			columns = EnumerationHelper.WhereNotNull<ColumnStyle>(columns);
			rows = EnumerationHelper.WhereNotNull<RowStyle>(rows);
			int colCount = 0, rowCount = 0;
			if (!EnumerationHelper.Any<ColumnStyle>(columns))
			{
				colCount = 1;
				panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));
			}
			else
			{
				foreach (ColumnStyle c in columns)
				{
					colCount++;
					panel.ColumnStyles.Add(c);
				}
			}
			if (!EnumerationHelper.Any<RowStyle>(rows))
			{
				rowCount = 1;
				panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
			}
			else
			{
				foreach (RowStyle c in rows)
				{
					rowCount++;
					panel.RowStyles.Add(c);
				}
			}
			
			panel.RowCount = rowCount;
			panel.ColumnCount = colCount;
		}
		protected static void InitializeTableLayoutPanel(TableLayoutPanel panel, IEnumerable<ColumnStyle> columns, params RowStyle[] rows) { InitializeTableLayoutPanel(panel, columns, rows as IEnumerable<RowStyle>); }
		protected static void InitializeTableLayoutPanel(TableLayoutPanel panel, IEnumerable<RowStyle> rows, IEnumerable<ColumnStyle> columns) { InitializeTableLayoutPanel(panel, columns, rows); }
		protected static void InitializeTableLayoutPanel(TableLayoutPanel panel, IEnumerable<RowStyle> rows, params ColumnStyle[] columns) { InitializeTableLayoutPanel(panel, columns, rows); }
		protected static void InitializeTableLayoutPanel(TableLayoutPanel panel, params ColumnStyle[] columns) { InitializeTableLayoutPanel(panel, columns, new RowStyle[0]); }
		protected static void InitializeTableLayoutPanel(TableLayoutPanel panel, params RowStyle[] rows) { InitializeTableLayoutPanel(panel, new ColumnStyle[0], rows); }
		protected static void InitializeTableLayoutPanel(TableLayoutPanel panel, IEnumerable<bool> columnsAreAutoSize, IEnumerable<bool> rowsAreAutoSize)
		{
			if (!EnumerationHelper.Any<bool, bool>(true, columnsAreAutoSize, EnumerationHelper.StructEqualsPredicateHandler<bool>))
				columnsAreAutoSize = EnumerationHelper.ReplaceLast<bool>(columnsAreAutoSize, true);
			if (!EnumerationHelper.Any<bool, bool>(true, rowsAreAutoSize, EnumerationHelper.StructEqualsPredicateHandler<bool>))
				rowsAreAutoSize = EnumerationHelper.ReplaceLast<bool>(rowsAreAutoSize, true);
			InitializeTableLayoutPanel(panel, EnumerationHelper.Select<float, bool, ColumnStyle>(GetAutoPercentage(columnsAreAutoSize), columnsAreAutoSize, new SelectValueStateHandler<float, bool, ColumnStyle>(BoolToColumnStyle)),
				EnumerationHelper.Select<float, bool, RowStyle>(GetAutoPercentage(rowsAreAutoSize), rowsAreAutoSize, new SelectValueStateHandler<float, bool, RowStyle>(BoolToRowStyle)));
		}
		protected static void InitializeTableLayoutPanel(TableLayoutPanel panel, IEnumerable<bool> columnsAreAutoSize, params bool[] rowsAreAutoSize)
		{
			InitializeTableLayoutPanel(panel, columnsAreAutoSize, rowsAreAutoSize as IEnumerable<bool>);
		}
		protected static void InitializeSingleRowTableLayoutPanel(TableLayoutPanel panel, IEnumerable<bool> columnsAreAutoSize) { InitializeTableLayoutPanel(panel, columnsAreAutoSize, new bool[0]); }
		protected static void InitializeSingleRowTableLayoutPanel(TableLayoutPanel panel, params bool[] columnsAreAutoSize) { InitializeTableLayoutPanel(panel, columnsAreAutoSize, new bool[0]); }
		protected static void InitializeSingleColumnTableLayoutPanel(TableLayoutPanel panel, IEnumerable<bool> rowsAreAutoSize){ InitializeTableLayoutPanel(panel, new bool[0], rowsAreAutoSize); }
		protected static void InitializeSingleColumnTableLayoutPanel(TableLayoutPanel panel, params bool[] rowsAreAutoSize){ InitializeTableLayoutPanel(panel, new bool[0], rowsAreAutoSize as IEnumerable<bool>); }
		#endregion
		#region CreateDataColumn
		protected DataColumn CreateDataColumn(string columnName, string caption, Type dataType, bool allowDBNull, bool unique)
		{
			DataColumn dataColumn = new DataColumn();
			dataColumn.AllowDBNull = allowDBNull;
			dataColumn.Caption = (String.IsNullOrEmpty(caption)) ? columnName : caption;
			dataColumn.ColumnName = columnName;
			dataColumn.DataType = (dataType == null) ? typeof(string) : dataType;
			dataColumn.Unique = unique;
			return dataColumn;
		}
		protected DataColumn CreateDataColumn(string columnName, string caption, Type dataType, bool allowDBNull) { return CreateDataColumn(columnName, caption, dataType, allowDBNull, false); }
		protected DataColumn CreateDataColumn(string columnName, Type dataType, bool allowDBNull, bool unique) { return CreateDataColumn(columnName, null, dataType, allowDBNull, unique); }
		protected DataColumn CreateDataColumn(string columnName, string caption, Type dataType) { return CreateDataColumn(columnName, caption, dataType, false); }
		protected DataColumn CreateDataColumn(string columnName, Type dataType, bool allowDBNull) { return CreateDataColumn(columnName, null, dataType, allowDBNull); }
		protected DataColumn CreateDataColumn(string columnName, string caption) { return CreateDataColumn(columnName, caption, null); }
		protected DataColumn CreateDataColumn(string columnName, Type dataType) { return CreateDataColumn(columnName, null as string, dataType); }
		protected DataColumn CreateDataColumn(string columnName) { return CreateDataColumn(columnName, null as string); }
		#endregion
		#region CreateDataGridView
		protected DataGridView CreateDataGridView(string name, bool readOnly, bool allowUserToAddRows, bool allowUserToDeleteRows, bool multiSelect, DataGridViewSelectionMode selectionMode)
		{
            DataGridView dataGridView = new DataGridView();
			dataGridView.Name = name;
			dataGridView.Dock = DockStyle.Fill;
			dataGridView.ReadOnly = readOnly;
			dataGridView.AllowUserToAddRows = allowUserToAddRows;
			dataGridView.AllowUserToDeleteRows = allowUserToDeleteRows;
			dataGridView.AutoGenerateColumns = false;
			dataGridView.AutoSize = true;
			dataGridView.MultiSelect = multiSelect;
			dataGridView.SelectionMode = selectionMode;
			return dataGridView;
		}
		protected DataGridView CreateDataGridView(string name, bool readOnly, bool allowUserToAddRows, bool allowUserToDeleteRows, bool multiSelect)
		{
			return CreateDataGridView(name, readOnly, allowUserToAddRows, allowUserToDeleteRows, multiSelect, DataGridViewSelectionMode.FullRowSelect);
		}
		protected DataGridView CreateDataGridView(string name, bool readOnly, bool allowUserToAddRows, bool allowUserToDeleteRows, DataGridViewSelectionMode selectionMode)
		{
			return CreateDataGridView(name, readOnly, allowUserToAddRows, allowUserToDeleteRows, false, selectionMode);
		}
		protected DataGridView CreateDataGridView(string name, bool readOnly, bool allowUserToAddRows, bool allowUserToDeleteRows)
		{
			return CreateDataGridView(name, readOnly, allowUserToAddRows, allowUserToDeleteRows, false);
		}
		protected DataGridView CreateDataGridView(string name, bool readOnly, bool allowUserToAddRows, DataGridViewSelectionMode selectionMode)
		{
			return CreateDataGridView(name, readOnly, allowUserToAddRows, false, selectionMode);
		}
		protected DataGridView CreateDataGridView(string name, bool readOnly, bool allowUserToAddRows) { return CreateDataGridView(name, readOnly, allowUserToAddRows, false); }
		protected DataGridView CreateDataGridView(string name, bool readOnly, DataGridViewSelectionMode selectionMode) { return CreateDataGridView(name, readOnly, false, selectionMode); }
		protected DataGridView CreateDataGridView(string name, bool readOnly) { return CreateDataGridView(name, readOnly, false); }
		protected DataGridView CreateDataGridView(string name) { return CreateDataGridView(name, false); }
		protected DataGridView CreateDataGridView(string name, DataGridViewSelectionMode selectionMode) { return CreateDataGridView(name, false, selectionMode); }
		#endregion
		#region CreateDataGridViewTextBoxColumn
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName, string headerText, bool readOnly, DataGridViewAutoSizeColumnMode autoSizeMode, DataGridViewColumnSortMode sortMode, bool hidden)
		{
            DataGridViewTextBoxColumn dataGridColumn = new DataGridViewTextBoxColumn();
			dataGridColumn.DataPropertyName = dataPropertyName;
			dataGridColumn.HeaderText = (headerText == null) ? dataPropertyName : headerText;
			dataGridColumn.ReadOnly = readOnly;
			dataGridColumn.AutoSizeMode = autoSizeMode;
			dataGridColumn.SortMode = sortMode;
			dataGridColumn.Visible = !hidden;
			return dataGridColumn;
		}
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName, string headerText, bool readOnly, DataGridViewAutoSizeColumnMode autoSizeMode, DataGridViewColumnSortMode sortMode)
		{
			return CreateDataGridViewTextBoxColumn(dataPropertyName, headerText, readOnly, autoSizeMode, sortMode, false);
		}
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName, string headerText, bool readOnly, DataGridViewAutoSizeColumnMode autoSizeMode)
		{
			return CreateDataGridViewTextBoxColumn(dataPropertyName, headerText, readOnly, autoSizeMode, DataGridViewColumnSortMode.NotSortable);
		}
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName, string headerText, bool readOnly, DataGridViewColumnSortMode sortMode)
		{
			return CreateDataGridViewTextBoxColumn(dataPropertyName, headerText, readOnly, DataGridViewAutoSizeColumnMode.AllCells, sortMode);
		}
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName, string headerText, bool readOnly, bool hidden)
		{
			return CreateDataGridViewTextBoxColumn(dataPropertyName, headerText, readOnly, DataGridViewAutoSizeColumnMode.AllCells, DataGridViewColumnSortMode.NotSortable, false);
		}
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName, string headerText, bool readOnly)
		{
			return CreateDataGridViewTextBoxColumn(dataPropertyName, headerText, readOnly, DataGridViewAutoSizeColumnMode.AllCells);
		}
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName, string headerText, DataGridViewColumnSortMode sortMode)
		{
			return CreateDataGridViewTextBoxColumn(dataPropertyName, headerText, false, sortMode);
		}
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName, string headerText) { return CreateDataGridViewTextBoxColumn(dataPropertyName, headerText, false); }
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName, DataGridViewColumnSortMode sortMode) { return CreateDataGridViewTextBoxColumn(dataPropertyName, null, sortMode); }
		protected DataGridViewTextBoxColumn CreateDataGridViewTextBoxColumn(string dataPropertyName) { return CreateDataGridViewTextBoxColumn(dataPropertyName, null); }
		protected DataGridViewTextBoxColumn CreateHiddenDataGridViewTextBoxColumn(string dataPropertyName) { return CreateDataGridViewTextBoxColumn(dataPropertyName, null, true, true); }
		#endregion
		#region CreateButton
		protected Button CreateButton(string name, string text, EventHandler click, DialogResult dialogResult, AnchorStyles anchor)
		{
            Button button = new Button();
			button.Name = name;
			button.Text = (text == null) ? name : text;
			button.Anchor = anchor;
			if (dialogResult != DialogResult.None)
				button.DialogResult = dialogResult;
			if (click != null)
				button.Click += click;
			return button;
		}
		protected Button CreateButton(string name, string text, EventHandler click, DialogResult dialogResult) { return CreateButton(name, text, click, dialogResult, AnchorStyles.Bottom | AnchorStyles.Top); }
		protected Button CreateButton(string name, string text, EventHandler click) { return CreateButton(name, text, click, DialogResult.None); }
		protected void TerminalButton_Click(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if (button == null || button.DialogResult == DialogResult.None)
				return;
			
			DialogResult = button.DialogResult;
			Close();
		}
		#endregion
	}
	public abstract class PasswordStorageTableLayoutFormBase : PasswordStorageFormBase
	{
		private TableLayoutPanel _outerTableLayoutPanel = null;
		protected TableLayoutPanel OuterTableLayoutPanel { get { return _outerTableLayoutPanel; } }
		protected PasswordStorageTableLayoutFormBase(string windowTitle, Size size, string name) : base(windowTitle, size, name) { }
		protected PasswordStorageTableLayoutFormBase(string windowTitle, string name) : base(windowTitle, name) { }
		protected override void OnFormInitializing()
		{
			_outerTableLayoutPanel = new TableLayoutPanel();
			_outerTableLayoutPanel.Name = "outerTableLayoutPanel";
			_outerTableLayoutPanel.SuspendLayout();
			Controls.Add(_outerTableLayoutPanel);
			try { OnOuterTableLayoutPanelInitializing(); }
			finally
			{
				_outerTableLayoutPanel.ResumeLayout(false);
				OnOuterTableLayoutPanelInitialized();
				_outerTableLayoutPanel.PerformLayout();
			}
		}
		protected virtual void OnOuterTableLayoutPanelInitializing()
		{
			_outerTableLayoutPanel.Dock = DockStyle.Fill;
            _outerTableLayoutPanel.AutoSize = true;
		}
		protected virtual void OnOuterTableLayoutPanelInitialized() { }
	}
	public class PasswordStorageViewForm : PasswordStorageTableLayoutFormBase
	{
		public PasswordStorageViewForm(string windowTitle, Size size, string name) : base(windowTitle, size, name) { }
		public PasswordStorageViewForm(string windowTitle, Size size) : this(windowTitle, size, null) { }
		public PasswordStorageViewForm(string windowTitle, string name) : base(windowTitle, name) { }
		public PasswordStorageViewForm(string windowTitle) : this(windowTitle, null) { }
		public PasswordStorageViewForm() : this(null) { }
	}
	public class PasswordStorageListingForm : PasswordStorageTableLayoutFormBase
	{
		#region Fields
		// NotifyIcon _openManagerIcon;
		private int? _selectedId = null;
		private DataGridView _mainDataGridView = null;
		private DataGridViewTextBoxColumn _idDataGridColumn = null;
		private DataGridViewTextBoxColumn _nameDataGridColumn = null;
		private DataGridViewTextBoxColumn _loginDataGridColumn = null;
		private DataGridViewTextBoxColumn _urlDataGridColumn = null;
		private DataGridViewTextBoxColumn _orderDataGridColumn = null;
		private Button _copyPasswordButton = null;
		private Button _moveUpButton = null;
		private Button _moveDownButton = null;
		private Button _editButton = null;
		private Button _duplicateButton = null;
		private Button _newButton = null;
		private Button _deleteButton = null;
		private Button _exitButton = null;
		private bool _orderChanged = false;
		private DataTable _dataSource = null;
		private XmlDocument _document = null;
		private DataColumn _idDataColumn = null, _nameDataColumn = null, _loginDataColumn = null, _urlDataColumn = null, _passwordDataColumn = null, _orderDataColumn = null;
		private DataGridViewCellStyle _currentRowCellStyle = null;
		#endregion
		#region Properties
		public int? SelectedId { get { return _selectedId; } set { _selectedId = value; } }
		public DataGridViewCellStyle CurrentRowCellStyle { get { return _currentRowCellStyle; } }
		public bool OrderChanged { get { return _orderChanged; } }
		public DataTable DataSource { get { return _dataSource; } }
		public XmlDocument Document { get { return _document; } set { _document = value; } }
		public DataColumn IdDataColumn { get { return _idDataColumn; } }
		public DataColumn NameDataColumn { get { return _nameDataColumn; } }
		public DataColumn LoginDataColumn { get { return _loginDataColumn; } }
		public DataColumn UrlDataColumn { get { return _urlDataColumn; } }
		public DataColumn PasswordDataColumn { get { return _passwordDataColumn; } }
		public DataColumn OrderDataColumn { get { return _orderDataColumn; } }
		#endregion
		#region Constructors
		public PasswordStorageListingForm(string windowTitle, Size size, string name) : base(windowTitle, size, name) { }
		public PasswordStorageListingForm(string windowTitle, Size size) : this(windowTitle, size, null) { }
		public PasswordStorageListingForm(string windowTitle, string name) : base(windowTitle, name) { }
		public PasswordStorageListingForm(string windowTitle) : this(windowTitle, null) { }
		public PasswordStorageListingForm() : this("Credential Listing") { }
		#endregion
		protected override void OnOuterTableLayoutPanelInitializing()
		{
			InitializeTableLayoutPanel(OuterTableLayoutPanel, new bool[] { true, false, false, false, false, false, false, false }, true, false);
			_dataSource = new DataTable("Credentials");
			Components.Add(_dataSource);
			_idDataColumn = CreateDataColumn("ID", typeof(int), false, true);
			_dataSource.Columns.Add(_idDataColumn);
			_nameDataColumn = CreateDataColumn("Name");
			_dataSource.Columns.Add(_nameDataColumn);
			_loginDataColumn = CreateDataColumn("Login");
			_dataSource.Columns.Add(_loginDataColumn);
			_urlDataColumn = CreateDataColumn("Url");
			_dataSource.Columns.Add(_urlDataColumn);
			_passwordDataColumn = CreateDataColumn("Password");
			_dataSource.Columns.Add(_passwordDataColumn);
			_orderDataColumn = CreateDataColumn("Order", typeof(int));
			_dataSource.Columns.Add(_orderDataColumn);
			
            _mainDataGridView = CreateDataGridView("mainDataGridView", DataGridViewSelectionMode.CellSelect);
            // _mainDataGridView = CreateDataGridView("mainDataGridView", DataGridViewSelectionMode.FullRowSelect);
            _idDataGridColumn = CreateHiddenDataGridViewTextBoxColumn("ID");
			_mainDataGridView.Columns.Add(_idDataGridColumn);
            _nameDataGridColumn = CreateDataGridViewTextBoxColumn("Name", DataGridViewColumnSortMode.Automatic);
			_mainDataGridView.Columns.Add(_nameDataGridColumn);
            _loginDataGridColumn = CreateDataGridViewTextBoxColumn("Login", DataGridViewColumnSortMode.Automatic);
			_mainDataGridView.Columns.Add(_loginDataGridColumn);
            _urlDataGridColumn = CreateDataGridViewTextBoxColumn("Url", DataGridViewColumnSortMode.Automatic);
			_mainDataGridView.Columns.Add(_urlDataGridColumn);
            _orderDataGridColumn = CreateDataGridViewTextBoxColumn("Order", DataGridViewColumnSortMode.Automatic);
			_mainDataGridView.Columns.Add(_orderDataGridColumn);
			_mainDataGridView.DataBindingComplete += DataGridView_DataBindingComplete;
			_mainDataGridView.SelectionChanged += DataGridView_SelectionChanged;
			_mainDataGridView.Sorted += DataGridView_SelectionChanged;
            _copyPasswordButton = CreateButton("copyPasswordButton", "Copy PW", PasswordButton_Click);
            _moveUpButton = CreateButton("moveUpButton", "Move Up", MoveUpButton_Click);
            _moveDownButton = CreateButton("moveDownButton", "Move Down", MoveDownButton_Click);
            _editButton = CreateButton("editButton", "Edit", TerminalButton_Click, DialogResult.Yes);
            _duplicateButton = CreateButton("duplicateButton", "Duplicate", null);
            _newButton = CreateButton("newButton", "New", TerminalButton_Click, DialogResult.No);
            _deleteButton = CreateButton("deleteButton", "Delete", TerminalButton_Click, DialogResult.Abort);
            _exitButton = CreateButton("exitButton", "Exit", TerminalButton_Click, DialogResult.Cancel);
			OuterTableLayoutPanel.Controls.Add(_mainDataGridView, 0, 0);
			OuterTableLayoutPanel.SetColumnSpan(_mainDataGridView, 8);
			OuterTableLayoutPanel.Controls.Add(_copyPasswordButton, 0, 1);
			OuterTableLayoutPanel.Controls.Add(_moveUpButton, 1, 1);
			OuterTableLayoutPanel.Controls.Add(_moveDownButton, 2, 1);
			OuterTableLayoutPanel.Controls.Add(_editButton, 3, 1);
			OuterTableLayoutPanel.Controls.Add(_duplicateButton, 4, 1);
			OuterTableLayoutPanel.Controls.Add(_newButton, 5, 1);
			OuterTableLayoutPanel.Controls.Add(_deleteButton, 6, 1);
			OuterTableLayoutPanel.Controls.Add(_exitButton, 7, 1);
			AcceptButton = _editButton;
			CancelButton = _exitButton;
			_dataSource.Clear();
			if (_document != null)
			{
				XmlNodeList xmlNodeList = _document.SelectNodes("/Credentials/Credential");
				for (int i = 0; i < xmlNodeList.Count; i++)
				{
					XmlElement xmlElement = xmlNodeList[i] as XmlElement;
					DataRow dataRow = _dataSource.NewRow();
					dataRow.BeginEdit();
					dataRow[_idDataColumn] = XmlConvert.ToInt32(GetAttributeText(xmlElement, "ID"));
					dataRow[_nameDataColumn] = GetElementText(xmlElement, "Name");
					dataRow[_loginDataColumn] = GetElementText(xmlElement, "Login");
					dataRow[_urlDataColumn] = GetElementText(xmlElement, "Url");
					dataRow[_passwordDataColumn] = GetElementText(xmlElement, "Password");
					dataRow[_orderDataColumn] = XmlConvert.ToInt32(GetAttributeText(xmlElement, "Order"));
					dataRow.EndEdit();
					_dataSource.Rows.Add(dataRow);
					dataRow.AcceptChanges();
				}
			}
			_dataSource.AcceptChanges();

			_mainDataGridView.DataSource = _dataSource;
		}
		public static string GetAttributeText(XmlElement xmlElement, string name)
		{
			if (xmlElement == null)
				return "";
			string s = xmlElement.GetAttribute(name);
			return (s == null) ? "" : s;
		}
		public static string GetElementText(XmlElement xmlElement, string xPath)
		{
			if (xmlElement == null)
				return "";
			XmlElement e = xmlElement.SelectSingleNode(xPath) as XmlElement;
			if (e == null || e.IsEmpty)
				return "";
			return e.InnerText.Trim();
		}
		public DataRow GetSelectedRow()
		{
			try
			{
				if (_mainDataGridView.CurrentCell != null && _mainDataGridView.CurrentCell.OwningRow != null && _mainDataGridView.CurrentCell.OwningRow.DataBoundItem != null)
					return (_mainDataGridView.CurrentCell.OwningRow.DataBoundItem as DataRowView).Row;
			}
			catch (Exception exception)
			{
				AddError(exception);
			}
			
			return null;
		}
		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			try
			{
				DataRow row = GetSelectedRow();
				if (row == null)
					SelectedId = null;
				else
					
					SelectedId = row[_idDataColumn] as int?;
				}
			catch (Exception exception)
			{
				AddError(exception);
			}
		}
		private void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			try
			{
				if (CurrentRowCellStyle == null)
				{
					_currentRowCellStyle = new DataGridViewCellStyle(_mainDataGridView.DefaultCellStyle);
					CurrentRowCellStyle.BackColor = Color.LightCoral;
					_mainDataGridView.Sort(_orderDataGridColumn, ListSortDirection.Ascending);
					DataGridView_SelectionChanged(_mainDataGridView, EventArgs.Empty);
				}
			}
			catch (Exception exception)
			{
				AddError(exception);
			}
		}
		private static int GetRowIndex(DataGridViewCell cell) { return cell.RowIndex; }
		private void DataGridView_SelectionChanged(object sender, EventArgs e)
		{
			try
			{
				if (CurrentRowCellStyle == null) { return; }
            
				IEnumerable<int> rowIndexes = EnumerationHelper.Select<DataGridViewCell, int>(EnumerationHelper.OfType<DataGridViewCell>(_mainDataGridView.SelectedCells), new SelectValueHandler<DataGridViewCell, int>(GetRowIndex));
				if (_mainDataGridView.SelectedCells.Count == 1)
				{
					foreach (DataGridViewRow r in _mainDataGridView.Rows)
					{
						if (r.Index != _mainDataGridView.CurrentCell.RowIndex)
							r.DefaultCellStyle = _mainDataGridView.DefaultCellStyle;
					}
					_mainDataGridView.CurrentCell.OwningRow.DefaultCellStyle = CurrentRowCellStyle;
					_editButton.Enabled = true;
					_deleteButton.Enabled = true;
					_duplicateButton.Enabled = true;
					DataRow dataRow = GetSelectedRow();
					if (dataRow == null) {
						_copyPasswordButton.Enabled = false;
						_moveUpButton.Enabled = false;
						_moveDownButton.Enabled = false;
					} else {
						_copyPasswordButton.Enabled = !String.IsNullOrEmpty(dataRow[_passwordDataColumn] as string);
						int rowIndex = _mainDataGridView.CurrentCell.OwningRow.Index;
						_moveUpButton.Enabled = (rowIndex > 0);
						_moveDownButton.Enabled = (rowIndex <= _mainDataGridView.RowCount);
					}
				} else {
					_copyPasswordButton.Enabled = false;
					_editButton.Enabled = false;
					_deleteButton.Enabled = false;
					_duplicateButton.Enabled = false;
					_moveUpButton.Enabled = false;
					_moveDownButton.Enabled = false;
				}
			}
			catch (Exception exception)
			{
				AddError(exception);
			}
		}
		private void PasswordButton_Click(object sender, EventArgs e)
		{
			try
			{
				DataRow dataRow = GetSelectedRow();
				if (dataRow == null) { return; }
				string password = dataRow[_passwordDataColumn] as string;
				if (!String.IsNullOrEmpty(password))
				{
					ScriptBlock sb = ScriptBlock.Create("$args[0] | ConvertTo-SecureString");
					Collection<PSObject> coll = sb.Invoke(password);
					SecureString ss = (coll.Count > 0 && coll[0] != null) ? coll[0].BaseObject as SecureString : null;
					if (ss != null)
					{
						using (ss)
						{
							PSCredential psCredential = new PSCredential(dataRow[_loginDataColumn] as string, ss);
							Clipboard.SetText(psCredential.GetNetworkCredential().Password, TextDataFormat.Text);
						}
					}
					else
						Clipboard.Clear();
				}
				else
					Clipboard.Clear();
			}
			catch (Exception exception)
			{
				AddError(exception);
			}
		}
		private void MoveUpButton_Click(object sender, EventArgs e)
		{
			try { SetDocumentRecordOrder(GetSelectedRow(), true); }
			catch (Exception exception) { AddError(exception); }
		}
		private void MoveDownButton_Click(object sender, EventArgs e)
		{
			try { SetDocumentRecordOrder(GetSelectedRow(), false); }
			catch (Exception exception) { AddError(exception); }
		}
		private void SetDocumentRecordOrder(DataRow dataRow, bool moveUp)
		{
			if (dataRow == null) { return; }
				int sourceId = (int)(dataRow[_idDataColumn]);
				int sourceOrder = (int)(dataRow[_orderDataColumn]);
				DataRow[] preceding = _dataSource.Select(String.Format("[ID] <> {0} AND [Order] = {1}", sourceId, sourceOrder), (moveUp) ? "ID DESC" : "ID ASC");
				if (preceding == null || preceding.Length == 0)
					preceding = _dataSource.Select(String.Format("[Order] {0} {1}", (moveUp) ? "<" : "?", sourceOrder), (moveUp) ? "Order DESC, ID DESC" : "Order ASC, ID ASC");
				if (preceding != null && preceding.Length > 0)
				{
					preceding[0].BeginEdit();
					try
					{
						dataRow.BeginEdit();
						try
						{
							int targetId = (int)(preceding[0][_idDataColumn]);
							int targetOrder = (int)(preceding[0][_orderDataColumn]);
							dataRow[_orderDataColumn] = targetOrder;
							preceding[0][_orderDataColumn] = sourceOrder;
							XmlElement xmlElement = _document.SelectSingleNode(String.Format("/Credentials/Credential[@ID=\"{0}\"]", sourceId)) as XmlElement;
							if (xmlElement != null)
								xmlElement.SetAttribute("Order", targetOrder.ToString());
							xmlElement = _document.SelectSingleNode(String.Format("/Credentials/Credential[@ID=\"{0}\"]", targetId)) as XmlElement;
							if (xmlElement != null)
								xmlElement.SetAttribute("Order", sourceOrder.ToString());
							_orderChanged = true;
							dataRow.EndEdit();
							preceding[0].EndEdit();
							dataRow.AcceptChanges();
							preceding[0].AcceptChanges();
							_dataSource.AcceptChanges();
							_mainDataGridView.Sort(_orderDataGridColumn, ListSortDirection.Ascending);
						}
						catch
						{
							dataRow.CancelEdit();
							throw;
						}
					}
					catch
					{
						preceding[0].CancelEdit();
						_dataSource.RejectChanges();
						throw;
					}
				}
		}
	}
}