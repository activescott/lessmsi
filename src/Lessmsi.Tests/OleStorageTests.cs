﻿using System.Diagnostics;
using System.Text;
using Xunit;

namespace LessMsi.Tests
{
	public class OleStorageTests: TestBase
	{
		[DebuggerHidden]
		static string FromRawBytes(byte[] data)
		{
			// Do not rely on encoding, build it manually!
			Assert.Equal(0, data.Length % 2);
			var sb = new StringBuilder(data.Length / 2);
			for (int n = 0; n < data.Length; n+= 2)
			{
				char ch = (char)(data[n] << 8);
				ch = (char)(ch | data[n + 1]);
				sb.Append(ch);
			}
			return sb.ToString();
		}

		[Fact]
		public void TestNameDecode()
		{
			// All names extracted from WPF2_32.msp using a c++ scratch project
			var testdata = new[]
			{
				new { Result = "_Columns", Data = new byte[]{0x48, 0x40, 0x3b, 0x3f, 0x43, 0xf2, 0x44, 0x38, 0x45, 0xb1} },	// STGTY_STREAM
				new { Result = "_Tables", Data = new byte[]{0x48, 0x40, 0x3f, 0x7f, 0x41, 0x64, 0x42, 0x2f, 0x48, 0x36 } },	// STGTY_STREAM
				new { Result = "T1ToU1", Data = new byte[]{0x00, 0x54, 0x00, 0x31, 0x00, 0x54, 0x00, 0x6f, 0x00, 0x55, 0x00, 0x31 } },	// STGTY_STORAGE
				new { Result = "#T1ToU1", Data = new byte[]{0x00, 0x23, 0x00, 0x54, 0x00, 0x31, 0x00, 0x54, 0x00, 0x6f, 0x00, 0x55, 0x00, 0x31 } },	// STGTY_STORAGE
				new { Result = "PCW_CAB_NetFX", Data = new byte[]{0x3b, 0x19, 0x47, 0xe0, 0x3a, 0x8c, 0x47, 0xcb, 0x42, 0x17, 0x3b, 0xf7, 0x48, 0x21 } },	// STGTY_STREAM
				new { Result = "_StringData", Data = new byte[]{0x48, 0x40, 0x3f, 0x3f, 0x45, 0x77, 0x44, 0x6c, 0x3b, 0x6a, 0x45, 0xe4, 0x48, 0x24 } },	// STGTY_STREAM
				new { Result = "_StringPool", Data = new byte[]{0x48, 0x40, 0x3f, 0x3f, 0x45, 0x77, 0x44, 0x6c, 0x3e, 0x6a, 0x44, 0xb2, 0x48, 0x2f } },	// STGTY_STREAM
				new { Result = "MsiPatchMetadata", Data = new byte[]{0x48, 0x40, 0x45, 0x96, 0x3e, 0x6c, 0x45, 0xe4, 0x42, 0xe6, 0x42, 0x16, 0x41, 0x37, 0x41, 0x27, 0x41, 0x37 } },	// STGTY_STREAM
				new { Result = "MsiPatchSequence", Data = new byte[]{0x48, 0x40, 0x45, 0x96, 0x3e, 0x6c, 0x45, 0xe4, 0x42, 0xe6, 0x42, 0x1c, 0x46, 0x34, 0x44, 0x68, 0x42, 0x26 } },	// STGTY_STREAM
				new { Result = "DigitalSignature", Data = new byte[]{0x00, 0x05, 0x00, 0x44, 0x00, 0x69, 0x00, 0x67, 0x00, 0x69, 0x00, 0x74, 0x00, 0x61, 0x00, 0x6c, 0x00, 0x53, 0x00, 0x69, 0x00, 0x67, 0x00, 0x6e, 0x00, 0x61, 0x00, 0x74, 0x00, 0x75, 0x00, 0x72, 0x00, 0x65 } },	// STGTY_STREAM
				new { Result = "SummaryInformation", Data = new byte[]{0x00, 0x05, 0x00, 0x53, 0x00, 0x75, 0x00, 0x6d, 0x00, 0x6d, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79, 0x00, 0x49, 0x00, 0x6e, 0x00, 0x66, 0x00, 0x6f, 0x00, 0x72, 0x00, 0x6d, 0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6f, 0x00, 0x6e } },	// STGTY_STREAM
			};

			foreach(var entry in testdata)
			{
				string tmp = FromRawBytes(entry.Data);
				Assert.Equal(entry.Result, OleStorage.OleStorageFile.DecodeName(tmp));
			}
		}
	}
}
