﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

// Modified to 
// 1) change sort to keep the "New Flight" entry at the end of the list.
// 2) sort null/empty times as greater than any existing time. 
// 3) sort string durations as if they were numeric.
// E. Woudenberg for GGC

// Courtesy:
//assembly: AssemblyTitle("Be.Timvw.Demo.SortableBindingList")]
//[assembly: AssemblyCompany("Tim Van Wassenhove")]
//[assembly: AssemblyProduct("Be.Timvw.Demo.SortableBindingList")]
//[assembly: AssemblyCopyright("Copyright © Tim Van Wassenhove 2008")]

namespace au.org.GGC {
    public class PropertyComparer<T> : IComparer<T> {
        private readonly IComparer comparer;
        private PropertyDescriptor propertyDescriptor;
        private int reverse;

        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction) {
            this.propertyDescriptor = property;
            Type comparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
            this.comparer = (IComparer)comparerForPropertyType.InvokeMember("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
            this.SetListSortDirection(direction);
        }

        #region IComparer<T> Members

        public int Compare(T x, T y) {
            if (((Flight)(Object)(x)).FlightNo == null) return 1;
            if (((Flight)(Object)(y)).FlightNo == null) return -1;
            object xval = this.propertyDescriptor.GetValue(x);
            object yval = this.propertyDescriptor.GetValue(y);
            if (this.propertyDescriptor.Name == "TakeOff" ||
                this.propertyDescriptor.Name == "TugDown" ||
                this.propertyDescriptor.Name == "GliderDown") {
                if (xval == null) return this.reverse * 1;
                if (yval == null) return this.reverse * -1;
            }
            if (this.propertyDescriptor.Name == "TowTime" ||
                this.propertyDescriptor.Name == "FlightTime") {
                int xi = ParseInt(xval);
                int yi = ParseInt(yval);
                return reverse * (xi - yi);
            }
            return this.reverse * this.comparer.Compare(xval, yval);
        }

        int ParseInt(object val) {
            int i = 0;
            if (!String.IsNullOrWhiteSpace((string)val))
                i = Int32.Parse(((string)val).Trim("+".ToCharArray()));
            return i;
        }

        #endregion

        private void SetPropertyDescriptor(PropertyDescriptor descriptor) {
            this.propertyDescriptor = descriptor;
        }

        private void SetListSortDirection(ListSortDirection direction) {
            this.reverse = direction == ListSortDirection.Ascending ? 1 : -1;
        }

        public void SetPropertyAndDirection(PropertyDescriptor descriptor, ListSortDirection direction) {
            this.SetPropertyDescriptor(descriptor);
            this.SetListSortDirection(direction);
        }
    }

    public class SortableBindingList<T> : BindingList<T> {
        private readonly Dictionary<Type, PropertyComparer<T>> comparers;
        private bool isSorted;
        private ListSortDirection listSortDirection;
        private PropertyDescriptor propertyDescriptor;

        public SortableBindingList()
            : base(new List<T>()) {
            this.comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        public SortableBindingList(IEnumerable<T> enumeration)
            : base(new List<T>(enumeration)) {
            this.comparers = new Dictionary<Type, PropertyComparer<T>>();
        }

        protected override bool SupportsSortingCore {
            get { return true; }
        }

        protected override bool IsSortedCore {
            get { return this.isSorted; }
        }

        protected override PropertyDescriptor SortPropertyCore {
            get { return this.propertyDescriptor; }
        }

        protected override ListSortDirection SortDirectionCore {
            get { return this.listSortDirection; }
        }

        protected override bool SupportsSearchingCore {
            get { return true; }
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction) {
            ApplySortCore(property, direction);
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction) {
            List<T> itemsList = (List<T>)this.Items;

            Type propertyType = property.PropertyType;
            PropertyComparer<T> comparer;
            if (!this.comparers.TryGetValue(propertyType, out comparer)) {
                comparer = new PropertyComparer<T>(property, direction);
                this.comparers.Add(propertyType, comparer);
            }

            comparer.SetPropertyAndDirection(property, direction);
            itemsList.Sort(comparer);

            this.propertyDescriptor = property;
            this.listSortDirection = direction;
            this.isSorted = true;

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore() {
            this.isSorted = false;
            this.propertyDescriptor = base.SortPropertyCore;
            this.listSortDirection = base.SortDirectionCore;

            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor property, object key) {
            int count = this.Count;
            for (int i = 0; i < count; ++i) {
                T element = this[i];
                if (property.GetValue(element).Equals(key)) {
                    return i;
                }
            }

            return -1;
        }
    }
}
