using LessMsi.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LessMsi
{
    /// <summary>
    /// Native/Win32 Methods to get around <see cref="System.IO.PathTooLongException"/>.
    /// Most of this based on the article series at https://blogs.msdn.microsoft.com/bclteam/2007/03/26/long-paths-in-net-part-2-of-3-long-path-workarounds-kim-hamilton/
    /// </summary>
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal class SECURITY_ATTRIBUTES
        {
            //internal unsafe byte* pSecurityDescriptor = (byte*)null;
            internal IntPtr pSecurityDescriptor;
            internal int nLength;
            internal int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct FILETIME
        {
            internal uint dwLowDateTime;
            internal uint dwHighDateTime;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WIN32_FIND_DATA
        {
            internal FileAttributes dwFileAttributes;
            internal FILETIME ftCreationTime;
            internal FILETIME ftLastAccessTime;
            internal FILETIME ftLastWriteTime;
            internal int nFileSizeHigh;
            internal int nFileSizeLow;
            internal int dwReserved0;
            internal int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string cFileName;
            // not using this
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            internal string cAlternate;
        }

        internal const int ERROR_ALREADY_EXISTS = 183;
        internal const int ERROR_PATH_NOT_FOUND = 3;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindFirstFile(string lpFileName, out
                                WIN32_FIND_DATA lpFindFileData);



        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool FindNextFile(IntPtr hFindFile, out
                                        WIN32_FIND_DATA lpFindFileData);


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FindClose(IntPtr hFindFile);

        internal static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        internal static int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        /// <summary>
        /// Should probably be using <see cref="CreateDirectoryHelper(string)"/>
        /// If not, see https://msdn.microsoft.com/en-us/library/windows/desktop/aa363855%28v=vs.85%29.aspx
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        internal static extern bool CreateDirectory(string path, SECURITY_ATTRIBUTES lpSecurityAttributes);

        [DllImport("kernel32.dll", EntryPoint="DeleteFile", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        internal static extern bool DeleteFile(string path);

        [DllImport("kernel32.dll", EntryPoint = "RemoveDirectory", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        internal static extern bool RemoveDirectory(string lpPathName);

        [DllImport("kernel32.dll", EntryPoint="SetFileAttributes",CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        internal static extern bool SetFileAttributes(string lpFileName, uint dwFileAttributes);

        /// <summary>
        /// Specified in Windows Headers for default maximum path. To go beyond this length you must prepend <see cref="LongPathPrefix"/> to the path.
        /// </summary>
        internal const int MAX_PATH = 260;
        /// <summary>
        /// This is the special prefix to prepend to paths to support up to 32,767 character paths.
        /// </summary>
        internal static readonly string LongPathPrefix = @"\\?\";
    }
}
