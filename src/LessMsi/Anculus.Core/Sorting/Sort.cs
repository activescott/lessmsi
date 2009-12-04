// 
// Copyright (c) 2006-2009 Ben Motmans
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Author(s):
//    Ben Motmans <ben.motmans@gmail.com>
//
using System;
using System.Collections.Generic;

namespace Anculus.Core
{
	//TODO: http://blog.quibb.org/2009/10/sorting-algorithm-shootout/

	/// <summary>
	/// Utility class to sort items inside a Generic List or array.
	/// </summary>
	public static class Sort
	{
		private static QuickSorter quicksorter;

		public static void QuickSort<T> (IList<T> list)
		{
			if (quicksorter == null)
				quicksorter = new QuickSorter ();

			quicksorter.Sort<T> (list);
		}

		public static void QuickSort<T> (IList<T> list, SortOrder sortOrder)
		{
			if (quicksorter == null)
				quicksorter = new QuickSorter ();

			quicksorter.Sort<T> (list, sortOrder);
		}

		public static void QuickSort<T> (IList<T> list, IComparer<T> comparer)
		{
			if (quicksorter == null)
				quicksorter = new QuickSorter ();

			quicksorter.Sort<T> (list, comparer);
		}

		public static void QuickSort<T> (IList<T> list, IComparer<T> comparer, SortOrder sortOrder)
		{
			if (quicksorter == null)
				quicksorter = new QuickSorter ();

			quicksorter.Sort<T> (list, comparer, sortOrder);
		}

		public static void QuickSort<T> (T[] array)
		{
			if (quicksorter == null)
				quicksorter = new QuickSorter ();

			quicksorter.Sort<T> (array);
		}

		public static void QuickSort<T> (T[] array, SortOrder sortOrder)
		{
			if (quicksorter == null)
				quicksorter = new QuickSorter ();

			quicksorter.Sort<T> (array, sortOrder);
		}

		public static void QuickSort<T> (T[] array, IComparer<T> comparer)
		{
			if (quicksorter == null)
				quicksorter = new QuickSorter ();

			quicksorter.Sort<T> (array, comparer);
		}

		public static void QuickSort<T> (T[] array, IComparer<T> comparer, SortOrder sortOrder)
		{
			if (quicksorter == null)
				quicksorter = new QuickSorter ();

			quicksorter.Sort<T> (array, comparer, sortOrder);
		}

		public static int BinarySearchIndex<T, U> (IList<T> list, IPropertyComparer<T, U> comparer, U property)
		{
			//method partially borrowed from Mono
			int min = 0;
			int max = list.Count - 1;
			int cmp = 0;

			while (min <= max) {
				int mid = (min + max) / 2;
				cmp = comparer.Compare (list[mid], property);

				if (cmp == 0)
					return mid;
				else if (cmp > 0)
					max = mid - 1;
				else
					min = mid + 1; // compensate for the rounding down
			}

			return ~min;
		}

		public static T BinarySearch<T, U> (IList<T> list, IPropertyComparer<T, U> comparer, U property)
		{
			int index = BinarySearchIndex<T, U> (list, comparer, property);
			if (index < 0)
				return default (T);
			return list[index];
		}
	}
}