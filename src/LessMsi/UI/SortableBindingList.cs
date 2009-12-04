using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace LessMsi.UI
{
    /// <summary>
    /// Implements a simple <see cref="System.ComponentModel.IBindingList"/> that provides sorting ability.
    /// </summary>
    /// <remarks>Useful to provide a sortable <see cref="System.Windows.Forms.DataGridView"/>.</remarks>
    class SortableBindingList<TItem> : BindingList<TItem>
    {
        private readonly IEnumerable<TItem> _originalItems;
        private PropertyDescriptor _sortProperty;
        private ListSortDirection _sortDirection;

        public SortableBindingList(IEnumerable<TItem> items)
        {
            _originalItems = items;
            ResetItems();
        }

        protected void ResetItems()
        {
            this.RaiseListChangedEvents = false;
            if (this.Items.Count > 0)
                this.Items.Clear();
            foreach (var i in _originalItems)
            {
                this.Add(i);
            }
            this.RaiseListChangedEvents = true;
            RaiseListSortingChanged();
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;
            Debug.WriteLine("ApplySortCore");
            RaiseListChangedEvents = false;
            Anculus.Core.Sort.QuickSort(this.Items, new PropertyComparer<TItem>(prop, direction));
            RaiseListChangedEvents = true;
            RaiseListSortingChanged();
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { Debug.WriteLine("SortPropertyCore"); return _sortProperty; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }
        protected override bool IsSortedCore
        {
            get { return _sortProperty != null; }
        }

        private void RaiseListSortingChanged()
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            ResetItems();
            _sortProperty = null;
            RaiseListSortingChanged();
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }
    }

    sealed class PropertyComparer<T> : IComparer<T>
    {
        private PropertyDescriptor _prop;
        private ListSortDirection _direction;

        public PropertyComparer(PropertyDescriptor prop, ListSortDirection direction)
        {
            if (prop == null)
                throw new ArgumentNullException("prop");
            _prop = prop;
            _direction = direction;
        }

        public int Compare(T x, T y)
        {
            var xValue = _prop.GetValue(x);
            var yValue = _prop.GetValue(y);
            var result = Comparer<object>.Default.Compare(xValue, yValue);
            if (_direction == ListSortDirection.Descending)
                result *= -1;
            return result;
        }
    }
}