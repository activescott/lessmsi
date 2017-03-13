// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2017 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using System;
using System.Diagnostics;
using System.Linq;
using LessIO;
using FileAccess = System.IO.FileAccess;
using FileMode = System.IO.FileMode;
using FileShare = System.IO.FileShare;
using BinaryReader = System.IO.BinaryReader;
using File = System.IO.File;
using Stream = System.IO.Stream;
using SeekOrigin = System.IO.SeekOrigin;
using System.IO.Packaging;
using System.Reflection;
using System.Text;
using System.Xml;
using Path = LessIO.Path;

namespace LessMsi.OleStorage
{
	/// <summary>
	/// Represents a Microsoft OLE Structured Storage File (of which MSIs are one of them, but so are things like the old Office binary documents).
	/// </summary>
	public sealed class OleStorageFile : IDisposable
	{
		private readonly Path _oleStorageFilePath;
		private readonly StorageInfo _storageRoot;
		private bool _isDisposed;

		public OleStorageFile(Path oleStorageFilePath)
		{
			int checkResult = NativeMethods.StgIsStorageFile(oleStorageFilePath.FullPathString);
			if (checkResult != 0)
				throw new ArgumentException("The specified file is not an OLE Structured Storage file.");
			_oleStorageFilePath = oleStorageFilePath;
			_storageRoot = GetStorageRoot(_oleStorageFilePath);
			_isDisposed = false;
		}

		~OleStorageFile()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// don't throw if we're in a finalizer
				ThrowIfDisposed();
			}
			var root = _storageRoot;
			if (root == null) return;
			CloseStorageRoot(root);
			_isDisposed = true;
		}

		public static void DebugDumpFile(Path fileName)
		{
			using (var storage = new OleStorageFile(fileName))
			{
				storage.Dump();
			}
		}

		private static StorageInfo GetStorageRoot(Path fileName)
		{
			var storageRoot = (StorageInfo)InvokeStorageRootMethod(null,
				"Open", fileName.FullPathString, FileMode.Open, FileAccess.Read, FileShare.Read);
			if (storageRoot == null)
			{
				throw new InvalidOperationException(string.Format("Unable to open \"{0}\" as a structured storage file.", fileName));
			}
			return storageRoot;
		}

		private static void CloseStorageRoot(StorageInfo storageRoot)
		{
			InvokeStorageRootMethod(storageRoot, "Close");
		}

		private static object InvokeStorageRootMethod(StorageInfo storageRoot, string methodName, params object[] methodArgs)
		{
			// from http://www.pinvoke.net/default.aspx/ole32/StgOpenStorageEx.html

			//We need the StorageRoot class to directly open an OSS file.  Unfortunately, it's internal.
			//So we'll have to use Reflection to access it.  This code was inspired by:
			//http://henbo.spaces.live.com/blog/cns!2E073207A544E12!200.entry
			//Note: In early WinFX CTPs the StorageRoot class was public because it was documented
			//here: http://msdn2.microsoft.com/en-us/library/aa480157.aspx

			Type storageRootType = typeof(StorageInfo).Assembly.GetType("System.IO.Packaging.StorageRoot", true, false);
			object result = storageRootType.InvokeMember(methodName,
				BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
				null, storageRoot, methodArgs);
			return result;
		}

		[DebuggerHidden]
		private void ThrowIfDisposed()
		{
			if (_isDisposed)
				throw new ObjectDisposedException(GetType().Name);
		}

		public StreamInfo[] GetStreams()
		{
			ThrowIfDisposed();
			return _storageRoot.GetStreams();
		}

		public StorageInfo StorageRoot
		{
			get
			{
				ThrowIfDisposed();
				return _storageRoot;
			}
		}

		private string BasePath
		{
			get
			{
				return _oleStorageFilePath.Parent.FullPathString;
			}
		}

		public void Dump()
		{
			var dumpfileName = Path.Combine(BasePath, Path.GetFileName(_oleStorageFilePath.FullPathString) + "_streams.xml").FullPathString;
			using (var writer = XmlWriter.Create(dumpfileName, new XmlWriterSettings { Indent = true }))
			{
				DumpStorage(StorageRoot, "root", writer);
			}
		}

		private void DumpStorage(StorageInfo storageInfo, string storageName, XmlWriter writer)
		{
			writer.WriteStartElement("storage");
			writer.WriteAttributeString("name", storageName);

			StorageInfo[] subStorages = storageInfo.GetSubStorages();
			foreach (StorageInfo subStorage in subStorages)
			{
				DumpStorage(subStorage, subStorage.Name, writer);
			}

			StreamInfo[] streams = storageInfo.GetStreams();
			foreach (StreamInfo stream in streams)
			{
				string hexData = ConvertStreamBytesToHex(stream);
				writer.WriteStartElement("stream");
				using (var rawStream = new BinaryReader(stream.GetStream(FileMode.Open, FileAccess.Read)))
				{
					var streamsBase = Path.Combine(BasePath, Path.GetFileName(_oleStorageFilePath.FullPathString) + "_streams");
					FileSystem.CreateDirectory(streamsBase);
					File.WriteAllBytes(Path.Combine(streamsBase, stream.Name).FullPathString, rawStream.ReadBytes((int)rawStream.BaseStream.Length));
					writer.WriteAttributeString("name", stream.Name);
					writer.WriteAttributeString("data", hexData);
					writer.WriteEndElement();
				}
			}

			writer.WriteEndElement();
		}

		private static string ConvertStreamBytesToHex(StreamInfo streamInfo)
		{
			using (var bits = streamInfo.GetStream(FileMode.Open, FileAccess.Read))
			{
				var sb = new StringBuilder();
				int currentRead;
				while ((currentRead = bits.ReadByte()) >= 0)
				{
					var currentByte = (byte)currentRead;
					sb.AppendFormat("{0:X2}", currentByte);
				}
				return sb.ToString();
			}
		}

		public static bool IsCabStream(StreamInfo si)
		{
			if (si == null)	
				throw new ArgumentNullException();
			using(var bits = si.GetStream())
			{
				return IsCabStream(bits);
			}
		}

		internal static bool IsCabStream(Stream bits)
		{
			byte[] cabHeaderBits = "MSCF".ToCharArray().Select(c => (byte)c).ToArray();
			var buffer = new byte[cabHeaderBits.Length];
			bits.Read(buffer, 0, buffer.Length);
			bits.Seek(0, SeekOrigin.Begin);
			return cabHeaderBits.SequenceEqual(buffer);
		}
	}
}