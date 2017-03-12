using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace LessMsi.OleStorage
{
	internal static class OleStorageHelper
	{
		// from http://www.pinvoke.net/default.aspx/ole32/StgOpenStorageEx.html

		public static void ReadFile(string fileName)
		{
			int checkResult = NativeMethods.StgIsStorageFile(fileName);
			if (checkResult != 0)
				throw new ArgumentException("The specified file is not an OLE Structured Storage file.");

			var storageRoot = GetStorageRoot(fileName);
			try
			{
				foreach (var strm in storageRoot.GetStreams())
				{
					Console.WriteLine(strm.Name);
				}
				var settings = new XmlWriterSettings();
				settings.Indent = true;
				WriteStorage(storageRoot, "root", XmlWriter.Create(@"c:\Users\scott\Downloads\boo.xml", settings));
			}
			finally
			{
				CloseStorageRoot(storageRoot);
			}
		}

		private static StorageInfo GetStorageRoot(string fileName)
		{
			var storageRoot = (StorageInfo)InvokeStorageRootMethod(null,
				"Open", fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
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

		private static void WriteStorage(StorageInfo storageInfo, string storageName, XmlWriter writer)
		{
			writer.WriteStartElement("storage");
			writer.WriteAttributeString("name", storageName);

			StorageInfo[] subStorages = storageInfo.GetSubStorages();
			foreach (StorageInfo subStorage in subStorages)
			{
				WriteStorage(subStorage, subStorage.Name, writer);
			}

			StreamInfo[] streams = storageInfo.GetStreams();
			foreach (StreamInfo stream in streams)
			{
				string hexData = ConvertStreamBytesToHex(stream);
				writer.WriteStartElement("stream");
				using (var rawStream = new BinaryReader(stream.GetStream(FileMode.Open, FileAccess.Read)))
				{
					//string base = Path.GetDirectoryName()
					
					string basePath = @"c:\Users\scott\Downloads\streams\";
					File.WriteAllBytes(Path.Combine(basePath, stream.Name), rawStream.ReadBytes((int)rawStream.BaseStream.Length));
					//writer.WriteAttributeString("name", stream.Name);
					//writer.WriteAttributeString("data", hexData);			
					//writer.WriteEndElement();
				}
			}

			writer.WriteEndElement();
		}

		private static string ConvertStreamBytesToHex(StreamInfo streamInfo)
		{
			using (Stream streamReader = streamInfo.GetStream(FileMode.Open, FileAccess.Read))
			{
				StringBuilder sb = new StringBuilder();
				int currentRead;
				while ((currentRead = streamReader.ReadByte()) >= 0)
				{
					byte currentByte = (byte)currentRead;
					sb.AppendFormat("{0:X2}", currentByte);
				}
				return sb.ToString();
			}
		}

	}
}
