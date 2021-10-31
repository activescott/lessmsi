using Xunit;

namespace LessMsi.Tests
{
	public class MspTests: TestBase
	{
		[Fact]
		public void MsXml5()
		{
			ExpectTables("msxml5.msp", new[] { "MsiPatchMetadata", "MsiPatchSequence" });
			// Cannot test properties yet, since they are internal in LessMsi.Gui!
			ExpectStreamCabFiles("msxml5.msp", true);
		}

		[Fact]
		public void WPF2_32()
		{
			ExpectTables("WPF2_32.msp", new[] { "MsiPatchMetadata", "MsiPatchSequence" });
			ExpectStreamCabFiles("WPF2_32.msp", true);
		}

		[Fact]
		public void SQL2008_AS()
		{
			ExpectTables("SQL2008_AS.msp", new[] { "MsiPatchSequence" });
			ExpectStreamCabFiles("SQL2008_AS.msp", true);
		}

	}
}
