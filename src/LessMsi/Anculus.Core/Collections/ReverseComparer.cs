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
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ReverseComparer<T> : IComparer<T>
	{
		private IComparer<T> comparer;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReverseComparer&lt;T&gt;"/> class.
		/// </summary>
		public ReverseComparer ()
			: this (Comparer<T>.Default)
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="ReverseComparer&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		public ReverseComparer (IComparer<T> comparer)
		{
			if (comparer == null)
				throw new ArgumentNullException ("comparer");

			this.comparer = comparer;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare (T x, T y)
		{
			return comparer.Compare (y, x);
		}
	}
}
