using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LteDev
{
    public class UriSegmentList : BindingList<UriSegment>
	{
		private object _syncRoot = new object();
		private ListSortDirection _sortDirection = ListSortDirection.Ascending;
		private List<UriSegment> _originalOrder = new List<UriSegment>();
		private PropertyDescriptor _sortProperty = null;
		private PropertyDescriptor _orderPropertyDescriptor;
		private PropertyDescriptor _namePropertyDescriptor;

		protected override ListSortDirection SortDirectionCore { get { return _sortDirection; } }

		protected override PropertyDescriptor SortPropertyCore { get { return _sortProperty; } }

		public UriSegmentList(IList<UriSegment> list) : base((list == null) ? new List<UriSegment>() : list.Where(i => i != null).ToList()) { Initialize(); }

		public UriSegmentList() : base() { Initialize(); }

		private void Initialize()
		{
			PropertyDescriptorCollection pc = TypeDescriptor.GetProperties(typeof(UriSegment));
			_orderPropertyDescriptor = pc.Cast<PropertyDescriptor>().FirstOrDefault(p => p.Name == UriSegment.PropertyName_Order);
			_namePropertyDescriptor = pc.Cast<PropertyDescriptor>().FirstOrDefault(p => p.Name == UriSegment.PropertyName_Name);
			foreach (UriSegment item in this)
			{
				if (!_originalOrder.Any(i => ReferenceEquals(i, item)))
					item.PropertyChanged += Item_PropertyChanged;	
				_originalOrder.Add(item);
			}
		}
		
		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender == null || !(sender is UriSegment))
				return;
			
			Monitor.Enter(_syncRoot);
			try
			{
				for (int i = 0; i < Items.Count; i++)
				{
					if (ReferenceEquals(Items[i], sender))
						ResetItem(i);
				}
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		private void _SetItemOrder(IEnumerable<UriSegment> enumerable)
		{
			int index = 0;
			foreach (UriSegment item in enumerable)
			{
				if (ReferenceEquals(item, Items[index]))
					continue;
				for (int i = 0; i < Items.Count; i++)
				{
					if (ReferenceEquals(item, Items[i]))
					{
						Items[i] = Items[index];
						Items[index] = item;
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, index, i));
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, i, index));
						break;
					}
				}
			}
		}
		
		protected override void ClearItems()
		{
			Monitor.Enter(_syncRoot);
			try
			{
				if (Items.Count == 0)
					return;
				
				base.ClearItems();
				
				if (Items.Count == 0)
				{
					while (_originalOrder.Count > 0)
					{
						UriSegment item = _originalOrder[0];
						_originalOrder.RemoveAt(0);
						if (!_originalOrder.Any(o => ReferenceEquals(o, item)))
							item.PropertyChanged -= Item_PropertyChanged;
					}
					return;
				}
				
				for (int i = 0; i < _originalOrder.Count; i++)
				{
					UriSegment item = _originalOrder[i];
					if (!Items.Any(o => ReferenceEquals(o, item)))
					{
						_originalOrder.RemoveAt(i);
						i--;
						if (!_originalOrder.Any(o => ReferenceEquals(o, item)))
							item.PropertyChanged -= Item_PropertyChanged;
					}
				}
			}
			finally { Monitor.Exit(_syncRoot); }
		}

		protected override void InsertItem(int index, UriSegment item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			
			Monitor.Enter(_syncRoot);
			try
			{
				UriSegment refItem = (index >=0 && index < Items.Count) ? Items[index] : null;
				int refIndex = (refItem == null) ? _originalOrder.Count : _originalOrder.TakeWhile(i => !ReferenceEquals(i, Items[index])).Count();
				base.InsertItem(index, item);
				if (!_originalOrder.Any(i => ReferenceEquals(i, item)))
					item.PropertyChanged += Item_PropertyChanged;
				if (refIndex < _originalOrder.Count)
					_originalOrder.Insert(refIndex, item);
				else
					_originalOrder.Add(item);
			}
			finally { Monitor.Exit(_syncRoot); }
		}

		protected override void SetItem(int index, UriSegment item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			
			Monitor.Enter(_syncRoot);
			try
			{
				UriSegment refItem = (index >=0 && index < Items.Count) ? Items[index] : null;
				int refIndex = (refItem == null) ? _originalOrder.Count : _originalOrder.TakeWhile(i => !ReferenceEquals(i, Items[index])).Count();
				base.SetItem(index, item);
				if (!_originalOrder.Any(i => ReferenceEquals(i, item)))
					item.PropertyChanged += Item_PropertyChanged;
				if (refIndex < _originalOrder.Count)
					_originalOrder[refIndex] = item;
				else
					_originalOrder.Add(item);
				if (refItem != null && !_originalOrder.Any(i => ReferenceEquals(i, refItem)))
					refItem.PropertyChanged -= Item_PropertyChanged;
			}
			finally { Monitor.Exit(_syncRoot); }
		}

		protected override object AddNewCore()
		{
			UriSegment item = new UriSegment();
			Monitor.Enter(_syncRoot);
			try
			{
				item.Order = _originalOrder.Count;
				foreach (UriSegment i in _originalOrder)
				{
					if (item.Order <= i.Order)
						item.Order = i.Order + 1;
				}
			}
			finally { Monitor.Exit(_syncRoot); }
			return item;
		}

		protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				if (prop == null || (_sortProperty != null && prop.Name == _sortProperty.Name && _sortDirection == direction))
					return;
				_sortDirection = direction;
				switch (prop.Name)
				{
					case UriSegment.PropertyName_Order:
						_sortProperty = _orderPropertyDescriptor;
						if (direction == ListSortDirection.Ascending)
							_SetItemOrder(this.OrderBy(p => p.Order));
						else
							_SetItemOrder(this.OrderByDescending(p => p.Order));
						break;
					case UriSegment.PropertyName_Name:
						_sortProperty = _namePropertyDescriptor;
						if (direction == ListSortDirection.Ascending)
							_SetItemOrder(this.OrderBy(p => p.Name));
						else
							_SetItemOrder(this.OrderByDescending(p => p.Name));
						break;
					default:
						_sortProperty = prop;
						if (direction == ListSortDirection.Ascending)
							_SetItemOrder(_originalOrder);
						else
							_SetItemOrder((_originalOrder as IEnumerable<UriSegment>).Reverse());
						break;
				}
			}
			finally { Monitor.Exit(_syncRoot); }
		}

		protected override int FindCore(PropertyDescriptor prop, object key)
		{
			if (prop == null || key == null)
				return -1;
			
			Monitor.Enter(_syncRoot);
			try
			{
				string s;
				UriSegment matching = null;
				try
				{
					switch (prop.Name)
					{
						case UriSegment.PropertyName_Order:
							int order = (key is int) ? (int)key : Convert.ToInt32(key);
							matching = _originalOrder.FirstOrDefault(i => i.Order == order);
							break;
						case UriSegment.PropertyName_Name:
							s = (key is string) ? (string)key : Convert.ToString(key);
							matching = _originalOrder.FirstOrDefault(i => i.Name == s);
							break;
					}
				}
				catch { }
				if (matching != null)
				{
					for (int i = 0; i < Count; i++)
					{
						if (ReferenceEquals(this[i], matching))
							return i;
					}
				}
			}
			finally { Monitor.Exit(_syncRoot); }
			
			return -1;
		}

		protected override bool IsSortedCore { get { return _sortProperty != null; } }

		protected override void RemoveSortCore()
		{
			Monitor.Enter(_syncRoot);
			try
			{
				_sortProperty = null;
				_sortDirection = ListSortDirection.Ascending;
				_SetItemOrder(_originalOrder);
			}
			finally { Monitor.Exit(_syncRoot); }
		}

		protected override bool SupportsChangeNotificationCore { get { return true; } }

		protected override bool SupportsSearchingCore { get { return true; } }

		protected override bool SupportsSortingCore {  get { return true; } }
		
		public void NormalizeOrderValues()
		{
			UriSegment[] itemArray = _originalOrder.OrderByDescending(i => i.Order).ToArray();
			for (int i = 0; i < itemArray.Length; i++)
				itemArray[i].Order = i;
		}
	}
}