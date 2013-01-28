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
// Copyright (c) 2004 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LibMSPackN;
using Microsoft.Tools.WindowsInstallerXml.Cab;
using Microsoft.Tools.WindowsInstallerXml.Msi;
using Misc.IO;

namespace LessMsi.Msi
{
    public class Wixtracts
    {
        #region class ExtractionProgress

        /// <summary>
        /// Provides progress information during an extraction operatation.
        /// </summary>
        public class ExtractionProgress : IAsyncResult
        {
            private string _currentFileName;
            private ExtractionActivity _activity;
            private ManualResetEvent _waitSignal;
            private AsyncCallback _callback;
            private int _totalFileCount;
            private int _filesExtracted;

            public ExtractionProgress(AsyncCallback progressCallback, int totalFileCount)
            {
                _activity = ExtractionActivity.Initializing;
                _currentFileName = "";
                _callback = progressCallback;
                _waitSignal = new ManualResetEvent(false);
                _totalFileCount = totalFileCount;
                _filesExtracted = 0;
            }

            internal void ReportProgress(ExtractionActivity activity, string currentFileName, int filesExtractedSoFar)
            {
                lock (this)
                {
                    _activity = activity;
                    _currentFileName = currentFileName;
                    _filesExtracted = filesExtractedSoFar;

                    if (this.IsCompleted)
                        _waitSignal.Set();

                    if (_callback != null)
                        _callback(this);
                }
            }

            /// <summary>
            /// The total number of files to be extracted for this operation.
            /// </summary>
            public int TotalFileCount
            {
                get
                {
                    lock (this)
                    {
                        return _totalFileCount;
                    }
                }
            }

            /// <summary>
            /// The number of files extracted so far
            /// </summary>
            public int FilesExtractedSoFar
            {
                get
                {
                    lock (this)
                    {
                        return _filesExtracted;
                    }
                }
            }


            /// <summary>
            /// If <see cref="Activity"/> is <see cref="ExtractionActivity.ExtractingFile"/>, specifies the name of the file being extracted.
            /// </summary>
            public string CurrentFileName
            {
                get
                {
                    lock (this)
                    {
                        return _currentFileName;
                    }
                }
            }

            /// <summary>
            /// Specifies the current activity.
            /// </summary>
            public ExtractionActivity Activity
            {
                get
                {
                    lock (this)
                    {
                        return _activity;
                    }
                }
            }

            #region IAsyncResult Members

            object IAsyncResult.AsyncState
            {
                get
                {
                    lock (this)
                    {
                        return this;
                    }
                }
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get
                {
                    lock (this)
                    {
                        return false;
                    }
                }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    lock (this)
                    {
                        return _waitSignal;
                    }
                }
            }

            public bool IsCompleted
            {
                get
                {
                    lock (this)
                    {
                        return this.Activity == ExtractionActivity.Complete;
                    }
                }
            }

            #endregion
        }

        #endregion

        #region enum ExtractionActivity

        /// <summary>
        /// Specifies the differernt available activities.
        /// </summary>
        public enum ExtractionActivity
        {
            Initializing,
            Uncompressing,
            ExtractingFile,
            Complete
        }

        #endregion

        public static void ExtractFiles(FileInfo msi, DirectoryInfo outputDir)
        {
            ExtractFiles(msi, outputDir, null, null);
        }

        /// <summary>
        /// Extracts the compressed files from the specified MSI file to the specified output directory.
        /// If specified, the list of <paramref name="filesToExtract"/> objects are the only files extracted.
        /// </summary>
        /// <param name="filesToExtract">The files to extract or null or empty to extract all files.</param>
        /// <param name="progressCallback">Will be called during during the operation with progress information, and upon completion. The argument will be of type <see cref="ExtractionProgress"/>.</param>
        public static void ExtractFiles(FileInfo msi, DirectoryInfo outputDir, MsiFile[] filesToExtract, AsyncCallback progressCallback)
        {
            if (msi == null)
                throw new ArgumentNullException("msi");
            if (outputDir == null)
                throw new ArgumentNullException("outputDir");

            int filesExtractedSoFar = 0;

            ExtractionProgress progress = null;
            Database msidb = new Database(msi.FullName, OpenDatabase.ReadOnly);
	        try
	        {
		        if (filesToExtract == null || filesToExtract.Length < 1)
			        filesToExtract = MsiFile.CreateMsiFilesFromMSI(msidb);

		        progress = new ExtractionProgress(progressCallback, filesToExtract.Length);

		        if (!msi.Exists)
		        {
			        Trace.WriteLine("File \'" + msi.FullName + "\' not found.");
			        progress.ReportProgress(ExtractionActivity.Complete, "", filesExtractedSoFar);
			        return;
		        }


		        progress.ReportProgress(ExtractionActivity.Initializing, "", filesExtractedSoFar);
		        outputDir.Create();


		        //map short file names to the msi file entry
		        var fileEntryMap = new Dictionary<string, MsiFile>(filesToExtract.Length, StringComparer.InvariantCulture);
		        foreach (var fileEntry in filesToExtract)
		        {
			        MsiFile existingFile = null;
			        if (fileEntryMap.TryGetValue(fileEntry.File, out existingFile))
			        {	//NOTE: This used to be triggered when we ignored case of file, but now we don't ignore case so this is unlikely to occur.
						// Differing only by case is not compliant with the msi specification but some installers do it (e.g. python, see issue 28).
				        Debug.Print("!!Found duplicate file using key {0}. The existing key was {1}", fileEntry.File, existingFile.File);
			        }
			        else
			        {
				        fileEntryMap.Add(fileEntry.File, fileEntry);
			        }
		        }

		        Debug.Assert(fileEntryMap.Count == filesToExtract.Length, "Duplicate files must have caused some files to not be in the map.");

		        var cabinets = CabsFromMsiToDisk(msidb, outputDir);
				foreach (CabInfo cabinfo in cabinets)
				{
					using (var cabDecompressor = new MSCabinet(cabinfo.LocalCabFile))
					{
						foreach (var compressedFile in cabDecompressor.GetFiles())
						{
							var entry = fileEntryMap[compressedFile.Filename];
							progress.ReportProgress(ExtractionActivity.ExtractingFile, entry.LongFileName, filesExtractedSoFar);
							DirectoryInfo targetDirectoryForFile = GetTargetDirectory(outputDir, entry.Directory);
							string destName = Path.Combine(targetDirectoryForFile.FullName, entry.LongFileName);
							if (File.Exists(destName))
							{
								Debug.Fail("output file already exists. We'll make it unique, but this is probably a strange msi or a bug in this program.");
								//make unique
// ReSharper disable HeuristicUnreachableCode
								Trace.WriteLine(string.Concat("Duplicate file found \'", destName, "\'"));
								int duplicateCount = 0;
								string uniqueName;
								do
								{
									uniqueName = string.Concat(destName, ".", "duplicate", ++duplicateCount);
								} while (File.Exists(uniqueName));
								destName = uniqueName;
// ReSharper restore HeuristicUnreachableCode
							}
							Trace.WriteLine(string.Concat("Extracting File \'", compressedFile.Filename, "\' to \'", destName, "\'"));
							compressedFile.ExtractTo(destName);
							filesExtractedSoFar++;
						}
					}
					//del local cab:
					File.Delete(cabinfo.LocalCabFile);
				}
	        }
	        finally
	        {
		        if (msidb != null)
			        msidb.Close();
		        if (progress != null)
			        progress.ReportProgress(ExtractionActivity.Complete, "", filesExtractedSoFar);
	        }
        }

        private static DirectoryInfo GetTargetDirectory(DirectoryInfo rootDirectory, MsiDirectory relativePath)
        {
            string fullPath = Path.Combine(rootDirectory.FullName, relativePath.GetPath());
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            return new DirectoryInfo(fullPath);

        }
		

		/// <summary>
		/// Extracts cab files from the specified MSIDB and puts them in the specified outputdir.
		/// </summary>
		/// <param name="msidb"></param>
		/// <param name="outputDir"></param>
		/// <returns></returns>
	    private static List<CabInfo> CabsFromMsiToDisk(Database msidb, DirectoryInfo outputDir)
	    {
		    const string query = "SELECT * FROM `Media`";
		    var localCabFiles = new List<CabInfo>();
		    using (View view = msidb.OpenExecuteView(query))
		    {
			    Record record;
			    while (view.Fetch(out record))
			    {
				    const int MsiInterop_Media_Cabinet = 4;
				    string cabSourceName = record[MsiInterop_Media_Cabinet];
				    if (string.IsNullOrEmpty(cabSourceName))
					    throw new IOException("Couldn't find media CAB file inside the MSI (bad media table?).");
				    if (0 < cabSourceName.Length)
				    {
					    if (cabSourceName.StartsWith("#"))
					    {
						    cabSourceName = cabSourceName.Substring(1);

						    // extract cabinet, then explode all of the files to a temp directory
						    string localCabFile = Path.Combine(outputDir.FullName, cabSourceName);

						    ExtractCabFromPackage(localCabFile, cabSourceName, msidb);
						    /* http://code.google.com/p/lessmsi/issues/detail?id=1
					 		 * apparently in some cases a file spans multiple CABs (VBRuntime.msi) so due to that we have get all CAB files out of the MSI and then begin extraction. Then after we extract everything out of all CAbs we need to release the CAB extractors and delete temp files.
							 * Thanks to Christopher Hamburg for explaining this!
					 		*/
						    var c = new CabInfo(localCabFile, cabSourceName);
						    localCabFiles.Add(c);
					    }
				    }
			    }
		    }
		    return localCabFiles;
	    }

	    class CabInfo
		{
			/// <summary>
			/// Name of the cab in the MSI.
			/// </summary>
			public string CabSourceName { get; set; }
			/// <summary>
			/// Path of the CAB on local disk after we pop it out of the msi.
			/// </summary>
			public string LocalCabFile { get; set; }

			public CabInfo(string localCabFile, string cabSourceName)
			{
				LocalCabFile = localCabFile;
				CabSourceName = cabSourceName;
			}
		}

        /// <summary>
        /// Write the Cab to disk.
        /// </summary>
        /// <param name="filePath">Specifies the path to the file to contain the stream.</param>
        /// <param name="cabName">Specifies the name of the file in the stream.</param>
        public static void ExtractCabFromPackage(string filePath, string cabName, Database inputDatabase)
        {
            using (View view = inputDatabase.OpenExecuteView(String.Concat("SELECT * FROM `_Streams` WHERE `Name` = '", cabName, "'")))
            {
                Record record;
                if (view.Fetch(out record))
                {
                    FileStream cabFilestream = null;
                    BinaryWriter writer = null;
                    try
                    {
                        cabFilestream = new FileStream(filePath, FileMode.Create);

                        // Create the writer for data.
                        writer = new BinaryWriter(cabFilestream);
                        
						var buf = new byte[1024*1024];
						int count;
						do
						{
							const int MsiInterop_Storages_Data = 2; //From wiX:Index to column name Data into Record for row in Msi Table Storages
							count = record.GetStream(MsiInterop_Storages_Data, buf, buf.Length);
							if (count > 0)
								writer.Write(buf, 0, count);
						} while (count > 0);
                    }
                    finally
                    {
                        if (writer != null)
                        {
                            writer.Close();
                        }

                        if (cabFilestream != null)
                        {
                            cabFilestream.Close();
                        }
                    }
                }
            }
        }
    }
}