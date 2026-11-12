import contextlib
import io
import os
from pathlib import Path
import sys
import tempfile
import unittest


assembly_dir = os.environ.get("LESSMSI_TEST_ASSEMBLY_DIR")
if assembly_dir:
    sys.path.insert(0, assembly_dir)
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

import LessMsi


class ExtractionTests(unittest.TestCase):
    fixture = (
        Path(__file__).resolve().parents[3]
        / "src/Lessmsi.Tests/TestFiles/MsiInput/ExtractOnlySomeFiles.msi"
    )

    def test_selected_filename_generator_and_progress_callback(self):
        selected = ["SampleSuiteExtensionTests.cs", "NUnitTests.config"]
        progress = []
        with tempfile.TemporaryDirectory() as directory:
            LessMsi.doExtraction(
                self.fixture,
                Path(directory),
                (Path(name) for name in selected),
                progress.append,
            )
            files = [path for path in Path(directory).rglob("*") if path.is_file()]
            self.assertEqual(sorted(path.name for path in files), sorted(selected))
            self.assertTrue(all(path.stat().st_size > 0 for path in files))
        self.assertTrue(progress)
        self.assertEqual(progress[-1].current, len(selected))
        self.assertEqual(progress[-1].total, len(selected))

    def test_none_and_empty_selection_extract_all_files(self):
        results = []
        for selection in (None, []):
            with tempfile.TemporaryDirectory() as directory:
                LessMsi.doExtraction(self.fixture, Path(directory), selection)
                results.append(
                    sorted(
                        str(path.relative_to(directory))
                        for path in Path(directory).rglob("*")
                        if path.is_file()
                    )
                )
        self.assertGreater(len(results[0]), 2)
        self.assertEqual(results[0], results[1])

    @unittest.skipUnless(
        hasattr(LessMsi, "doExtractionWithTqdmProgressBar"), "tqdm is not installed"
    )
    def test_tqdm_extraction_forwards_progress(self):
        progress = []
        with tempfile.TemporaryDirectory() as directory, contextlib.redirect_stderr(io.StringIO()):
            LessMsi.doExtractionWithTqdmProgressBar(
                self.fixture,
                Path(directory),
                [Path("NUnitTests.config")],
                progress.append,
            )
            files = [path.name for path in Path(directory).rglob("*") if path.is_file()]
            self.assertEqual(files, ["NUnitTests.config"])
        self.assertTrue(progress)
        self.assertEqual(progress[-1].current, 1)


if __name__ == "__main__":
    unittest.main()
