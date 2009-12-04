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
	public abstract class AbstractSorter : ISorter
	{
		public virtual void Sort<T> (IList<T> list)
		{
			Sort<T> (list, Comparer<T>.Default);
		}

		public virtual void Sort<T> (IList<T> list, SortOrder sortOrder)
		{
			if (sortOrder == SortOrder.Ascending)
				Sort<T> (list, Comparer<T>.Default);
			else
				Sort<T> (list, new ReverseComparer<T> (Comparer<T>.Default));
		}

		public virtual void Sort<T> (IList<T> list, IComparer<T> comparer, SortOrder sortOrder)
		{
			if (comparer == null)
				throw new ArgumentNullException ("comparer");

			if (sortOrder == SortOrder.Ascending)
				Sort<T> (list, comparer);
			else
				Sort<T> (list, new ReverseComparer<T> (comparer));
		}

		public abstract void Sort<T> (IList<T> list, IComparer<T> comparer);

		public virtual void Sort<T> (T[] array)
		{
			Sort<T> (array, Comparer<T>.Default);
		}

		public virtual void Sort<T> (T[] array, SortOrder sortOrder)
		{
			if (sortOrder == SortOrder.Ascending)
				Sort<T> (array, Comparer<T>.Default);
			else
				Sort<T> (array, new ReverseComparer<T> (Comparer<T>.Default));
		}

		public virtual void Sort<T> (T[] array, IComparer<T> comparer, SortOrder sortOrder)
		{
			if (comparer == null)
				throw new ArgumentNullException ("comparer");

			if (sortOrder == SortOrder.Ascending)
				Sort<T> (array, comparer);
			else
				Sort<T> (array, new ReverseComparer<T> (comparer));
		}

		public abstract void Sort<T> (T[] array, IComparer<T> comparer);

		protected static void Swap<T> (IList<T> list, int left, int right)
		{
			T swap = list[left];
			list[left] = list[right];
			list[right] = swap;
		}

		protected static void Swap<T> (T[] array, int left, int right)
		{
			T swap = array[left];
			array[left] = array[right];
			array[right] = swap;
		}
	}
}
