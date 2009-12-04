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
	public sealed class QuickSorter : AbstractSorter
	{
		public override void Sort<T> (IList<T> list, IComparer<T> comparer)
		{
			if (list == null)
				throw new ArgumentNullException ("list");
			if (comparer == null)
				throw new ArgumentNullException ("comparer");

			if (list.Count <= 1) return;

			Sort<T> (list, comparer, 0, list.Count - 1);
		}

		private static void Sort<T> (IList<T> list, IComparer<T> comparer, int lower, int upper)
		{
			if (lower < upper) {
				int split = Pivot<T> (list, comparer, lower, upper);
				Sort<T> (list, comparer, lower, split - 1);
				Sort<T> (list, comparer, split + 1, upper);
			}
		}

		private static int Pivot<T> (IList<T> list, IComparer<T> comparer, int lower, int upper)
		{
			int left = lower + 1;
			T pivot = list[lower];
			int right = upper;

			while (left <= right) {
				while ((left <= right) && (comparer.Compare (list[left], pivot) <= 0))
					++left;

				while ((left <= right) && (comparer.Compare (list[right], pivot) > 0))
					--right;

				if (left < right) {
					Swap<T> (list, left, right);
					++left;
					--right;
				}
			}

			Swap<T> (list, lower, right);
			return right;
		}

		public override void Sort<T> (T[] array, IComparer<T> comparer)
		{
			if (array == null)
				throw new ArgumentNullException ("array");
			if (comparer == null)
				throw new ArgumentNullException ("comparer");

			if (array.Length <= 1) return;

			Sort<T> (array, comparer, 0, array.Length - 1);
		}

		private static void Sort<T> (T[] array, IComparer<T> comparer, int lower, int upper)
		{
			if (lower < upper) {
				int split = Pivot<T> (array, comparer, lower, upper);
				Sort<T> (array, comparer, lower, split - 1);
				Sort<T> (array, comparer, split + 1, upper);
			}
		}

		private static int Pivot<T> (T[] array, IComparer<T> comparer, int lower, int upper)
		{
			int left = lower + 1;
			T pivot = array[lower];
			int right = upper;

			while (left <= right) {
				while ((left <= right) && (comparer.Compare (array[left], pivot) <= 0))
					++left;

				while ((left <= right) && (comparer.Compare (array[right], pivot) > 0))
					--right;

				if (left < right) {
					Swap<T> (array, left, right);
					++left;
					--right;
				}
			}

			Swap<T> (array, lower, right);
			return right;
		}
	}
}