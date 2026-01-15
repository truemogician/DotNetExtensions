using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TrueMogician.Extensions.IO;

public static class PInvoke {
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ByHandleFileInformation {
        public uint dwFileAttributes;
        public long ftCreationTime;
        public long ftLastAccessTime;
        public long ftLastWriteTime;
        public uint dwVolumeSerialNumber;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint nNumberOfLinks;
        public uint nFileIndexHigh;
        public uint nFileIndexLow;
    }

	public enum FileInfoByHandleClass {
        FileBasicInfo = 0,
        FileStandardInfo = 1,
        FileNameInfo = 2,
        FileRenameInfo = 3,
        FileDispositionInfo = 4,
        FileAllocationInfo = 5,
        FileEndOfFileInfo = 6,
        FileStreamInfo = 7,
        FileCompressionInfo = 8,
        FileAttributeTagInfo = 9,
        FileIdBothDirectoryInfo = 10,
        FileIdBothDirectoryRestartInfo = 11,
        FileIoPriorityHintInfo = 12,
        FileRemoteProtocolInfo = 13,
        FileFullDirectoryInfo = 14,
        FileFullDirectoryRestartInfo = 15,
        FileStorageInfo = 16,
        FileAlignmentInfo = 17,
        FileIdInfo = 18,
        FileIdExtdDirectoryInfo = 19,
        FileIdExtdDirectoryRestartInfo = 20,
        MaximumFileInfoByHandleClass = 21
    }

    [StructLayout(LayoutKind.Sequential)]
	public struct FileBasicInfo {
        public long CreationTime;
        public long LastAccessTime;
        public long LastWriteTime;
        public long ChangeTime;
        public uint FileAttributes;
    }

#if WINDOWS
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern SafeFileHandle CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile
    );

    [DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool GetFileInformationByHandle(
        SafeFileHandle hFile,
        out ByHandleFileInformation lpFileInformation
    );

    [DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool GetFileInformationByHandleEx(
        SafeFileHandle hFile,
        FileInfoByHandleClass fileInformationClass,
        IntPtr lpFileInformation,
        uint dwBufferSize
    );

    [DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool SetFileInformationByHandle(
        SafeFileHandle hFile,
        FileInfoByHandleClass fileInformationClass,
        IntPtr lpFileInformation,
        uint dwBufferSize
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern unsafe bool SetFileTime(
        SafeFileHandle hFile,
        long* lpCreationTime,
        long* lpLastAccessTime,
        long* lpLastWriteTime
    );
#endif

	public static void ThrowLastWin32Error() => throw new Win32Exception(Marshal.GetLastWin32Error());
}