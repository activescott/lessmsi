__all__=["doExtraction", "ExtrProgress"]
import typing
import clr
clr.AddReference("LessMsi.core")
clr.AddReference("LessIO")
import LessIO
from LessMsi.Msi import Wixtracts
from pathlib import Path

__license__="MIT"
__copyright__=r"""
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Copyright (c) 2004 Scott Willeke (http://scott.willeke.com)

Authors:
	Scott Willeke (scott@willeke.com)
"""

class ExtrProgress:
	"""Represents progress of unpacking"""
	__slots__=("current", "total", "fileName")
	def __init__(self, current:int, total:int, fileName:Path):
		self.current=current
		self.total=total
		self.fileName=fileName
	def __repr__(self):
		return self.__class__.__name__+"("+", ".join((str(self.current), str(self.total), str(self.fileName)))+")"

from time import sleep
def doExtraction(msiFileName:Path, outDirName:Path="", filesToExtract:typing.Iterable[Path]=None, progressCallback=None):
	"""Extracts files from a .msi.
	See https://github.com/activescott/lessmsi/blob/master/src/LessMsi.Cli/Program.cs#L104 for more info
	"""
	
	msiFileName=str(Path(msiFileName).absolute())
	outDirName=str(Path(outDirName).absolute())
	if filesToExtract:
		filesToExtract=(str(Path(outDirName).absolute()) for f in filesToExtract)
	msiFile = LessIO.Path(msiFileName)
	if progressCallback:
		def cb(progress:Wixtracts.ExtractionProgress):
			#progress.Activity
			return progressCallback(ExtrProgress(progress.FilesExtractedSoFar, progress.TotalFileCount, Path(progress.CurrentFileName)))
		cb=clr.System.AsyncCallback(cb)
	else:
		cb=None
	Wixtracts.ExtractFiles(msiFile, outDirName, filesToExtract, cb)

try:
	from tqdm import tqdm
	def doExtractionWithTqdmProgressBar(msiFileName:Path, outDirName:Path="", filesToExtract:typing.Iterable[Path]=None, progressCallback=None):
		"""Extracts files from a .msi showing a tqdm-based progressbar."""
		prev=0
		with tqdm(unit="file") as pb:
			def cb(progr:ExtrProgress):
				nonlocal pb, prev
				pb.desc=str(progr.fileName)
				delta=progr.current-prev
				prev=progr.current
					pb.total=progr.total
				pb.update(delta)
				if progressCallback:
					return progressCallback(progr)
			doExtraction(msiFileName, outDirName=outDirName, filesToExtract=filesToExtract, progressCallback=cb)
	__all__.append(doExtractionWithTqdmProgressBar.__name__)
except ImportError:
	pass
