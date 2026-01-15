using System;
using System.IO;
#if WINDOWS
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32.SafeHandles;
#endif

namespace TrueMogician.Extensions.IO;

using static PInvoke;

#if WINDOWS
/// <summary>
///     Provides properties and instance methods for accessing and modifying metadata of a file system entry with minimum
///     permissions and overhead. This class cannot be inherited.
/// </summary>
public sealed class EntryMetadata : IEquatable<EntryMetadata> {
	private const uint FILE_READ_ATTRIBUTES = 0x0080;
	private const uint FILE_WRITE_ATTRIBUTES = 0x0100;
	private const uint FILE_SHARE_READ = 1;
	private const uint FILE_SHARE_WRITE = 2;
	private const uint FILE_SHARE_DELETE = 4;
	private const uint OPEN_EXISTING = 3;
	private const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

	public EntryMetadata(string path) {
		if (path is null)
			throw new ArgumentNullException(nameof(path));
		FullName = Path.GetFullPath(path);
	}

	public EntryMetadata(FileSystemInfo info) => FullName = info.FullName;

	/// <inheritdoc cref="FileSystemInfo.FullName" />
	public string FullName { get; }

	/// <summary>
	///     Gets the basic metadata information of the file system entry.
	/// </summary>
	public EntryBasicInfo BasicInfo => GetFileBasicInfo();

	/// <inheritdoc cref="FileSystemInfo.Attributes" />
	public FileAttributes Attributes {
		get {
			var info = GetFileBasicInfo();
			return (FileAttributes)info.FileAttributes;
		}
		set {
			var current = GetFileBasicInfo();
			current.FileAttributes = (uint)value;
			SetFileBasicInfo(current);
		}
	}

	/// <inheritdoc cref="FileSystemInfo.CreationTime" />
	public DateTime CreationTime {
		get => CreationTimeUtc.ToLocalTime();
		set => CreationTimeUtc = value.ToUniversalTime();
	}

	/// <inheritdoc cref="FileSystemInfo.LastAccessTime" />
	public DateTime LastAccessTime {
		get => LastAccessTimeUtc.ToLocalTime();
		set => LastAccessTimeUtc = value.ToUniversalTime();
	}

	/// <inheritdoc cref="FileSystemInfo.LastWriteTime" />
	public DateTime LastWriteTime {
		get => LastWriteTimeUtc.ToLocalTime();
		set => LastWriteTimeUtc = value.ToUniversalTime();
	}

	/// <summary>
	///     Gets the time when the current file or directory was changed.
	/// </summary>
	public DateTime ChangeTime => ChangeTimeUtc.ToLocalTime();

	/// <inheritdoc cref="FileSystemInfo.CreationTimeUtc" />
	public DateTime CreationTimeUtc {
		get => GetTimeUtc(EntryTimestamp.Creation);
		set => SetTimeUtc(EntryTimestamp.Creation, value);
	}

	/// <inheritdoc cref="FileSystemInfo.LastAccessTimeUtc" />
	public DateTime LastAccessTimeUtc {
		get => GetTimeUtc(EntryTimestamp.LastAccess);
		set => SetTimeUtc(EntryTimestamp.LastAccess, value);
	}

	/// <inheritdoc cref="FileSystemInfo.LastWriteTimeUtc" />
	public DateTime LastWriteTimeUtc {
		get => GetTimeUtc(EntryTimestamp.LastWrite);
		set => SetTimeUtc(EntryTimestamp.LastWrite, value);
	}

	/// <summary>
	///     Gets the time, in coordinated universal time (UTC), when the current file or directory was changed.
	/// </summary>
	public DateTime ChangeTimeUtc => GetTimeUtc(EntryTimestamp.Change);

	/// <summary>
	///     Gets the specified timestamp.
	/// </summary>
	/// <inheritdoc cref="GetTimeUtc" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateTime GetTime(EntryTimestamp t) => GetTimeUtc(t).ToLocalTime();

	/// <summary>
	///     Gets the specified timestamp in UTC.
	/// </summary>
	/// <param name="t">The timestamp to retrieve.</param>
	/// <exception cref="ArgumentOutOfRangeException" />
	/// <exception cref="Win32Exception" />
	public DateTime GetTimeUtc(EntryTimestamp t) {
		var info = GetFileBasicInfo();
		return t switch {
			EntryTimestamp.Creation   => DateTime.FromFileTimeUtc(info.CreationTime),
			EntryTimestamp.LastAccess => DateTime.FromFileTimeUtc(info.LastAccessTime),
			EntryTimestamp.LastWrite  => DateTime.FromFileTimeUtc(info.LastWriteTime),
			EntryTimestamp.Change     => DateTime.FromFileTimeUtc(info.ChangeTime),
			_                         => throw new ArgumentOutOfRangeException(nameof(t))
		};
	}

	/// <summary>
	///     Sets the specified timestamp.
	/// </summary>
	/// <inheritdoc cref="SetTimeUtc" />
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetTime(EntryTimestamp t, DateTime value) => SetTimeUtc(t, value.ToUniversalTime());

	/// <summary>
	///     Sets the specified timestamp in UTC.
	/// </summary>
	/// <param name="utcValue">The new UTC timestamp.</param>
	/// <inheritdoc cref="GetTimeUtc" />
	public void SetTimeUtc(EntryTimestamp t, DateTime utcValue) {
		if (t == EntryTimestamp.Change)
			throw new NotSupportedException("ChangeTime is read-only.");
		long fileTime = utcValue.ToFileTimeUtc();
		unsafe {
			long* c = t == EntryTimestamp.Creation ? &fileTime : null;
			long* a = t == EntryTimestamp.LastAccess ? &fileTime : null;
			long* w = t == EntryTimestamp.LastWrite ? &fileTime : null;
			using var handle = OpenHandle(FILE_WRITE_ATTRIBUTES);
			if (!SetFileTime(handle, c, a, w))
				ThrowLastWin32Error();
		}
	}

	/// <summary>
	///     Set all three timestamps. Use <see langword="null" /> to leave a timestamp unchanged.
	/// </summary>
	/// <param name="creation">The new creation time.</param>
	/// <param name="access">The new last access time.</param>
	/// <param name="write">The new last write time.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetTimes(DateTime? creation, DateTime? access, DateTime? write)
		=> SetTimesUtc(creation?.ToUniversalTime(), access?.ToUniversalTime(), write?.ToUniversalTime());

	/// <summary>
	///     Set all three timestamps in UTC. Use <see langword="null" /> to leave a timestamp unchanged.
	/// </summary>
	/// <param name="creationUtc">The new creation time in UTC.</param>
	/// <param name="accessUtc">The new last access time in UTC.</param>
	/// <param name="writeUtc">The new last write time in UTC.</param>
	public void SetTimesUtc(DateTime? creationUtc, DateTime? accessUtc, DateTime? writeUtc) {
		unsafe {
			long cVal = creationUtc?.ToFileTimeUtc() ?? 0;
			long aVal = accessUtc?.ToFileTimeUtc() ?? 0;
			long wVal = writeUtc?.ToFileTimeUtc() ?? 0;
			long* cPtr = creationUtc.HasValue ? &cVal : null;
			long* aPtr = accessUtc.HasValue ? &aVal : null;
			long* wPtr = writeUtc.HasValue ? &wVal : null;
			using var handle = OpenHandle(FILE_WRITE_ATTRIBUTES);
			if (!SetFileTime(handle, cPtr, aPtr, wPtr))
				ThrowLastWin32Error();
		}
	}

	private SafeFileHandle OpenHandle(uint desiredAccess) {
		var handle = CreateFile(
			FullName,
			desiredAccess,
			FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
			IntPtr.Zero,
			OPEN_EXISTING,
			FILE_FLAG_BACKUP_SEMANTICS,
			IntPtr.Zero
		);
		if (handle.IsInvalid)
			ThrowLastWin32Error();
		return handle;
	}

	private unsafe FileBasicInfo GetFileBasicInfo() {
		FileBasicInfo info;
		using var handle = OpenHandle(FILE_READ_ATTRIBUTES);
		if (!GetFileInformationByHandleEx(handle, FileInfoByHandleClass.FileBasicInfo, (IntPtr)(&info), (uint)sizeof(FileBasicInfo)))
			ThrowLastWin32Error();
		return info;
	}

	private unsafe void SetFileBasicInfo(FileBasicInfo info) {
		using var handle = OpenHandle(FILE_WRITE_ATTRIBUTES);
		if (!SetFileInformationByHandle(handle, FileInfoByHandleClass.FileBasicInfo, (IntPtr)(&info), (uint)sizeof(FileBasicInfo)))
			ThrowLastWin32Error();
	}

	#region Boilerplate
	public override string ToString() => FullName;

	public bool Equals(EntryMetadata? other) => other is not null && (ReferenceEquals(this, other) || FullName == other.FullName);

	public override bool Equals(object? obj) => obj is EntryMetadata other && Equals(other);

	public override int GetHashCode() => FullName.GetHashCode();

	public static implicit operator EntryMetadata(FileSystemInfo info) => new(info);

	public static explicit operator string(EntryMetadata info) => info.FullName;

	public static explicit operator EntryMetadata(string path) => new(path);
	#endregion
}
#endif

public enum EntryTimestamp : byte {
	Creation,
	LastAccess,
	LastWrite,
	Change
}

/// <summary>
///     A struct that contains basic metadata information of a file system entry.
/// </summary>
public struct EntryBasicInfo {
	public DateTime CreationTimeUtc;
	public DateTime LastAccessTimeUtc;
	public DateTime LastWriteTimeUtc;
	public DateTime ChangeTimeUtc;
	public FileAttributes Attributes;

	public DateTime CreationTime {
		get => CreationTimeUtc.ToLocalTime();
		set => CreationTimeUtc = value.ToUniversalTime();
	}

	public DateTime LastAccessTime {
		get => LastAccessTimeUtc.ToLocalTime();
		set => LastAccessTimeUtc = value.ToUniversalTime();
	}

	public DateTime LastWriteTime {
		get => LastWriteTimeUtc.ToLocalTime();
		set => LastWriteTimeUtc = value.ToUniversalTime();
	}

	public DateTime ChangeTime => ChangeTimeUtc.ToLocalTime();

	public static implicit operator EntryBasicInfo(FileBasicInfo info) => new() {
		CreationTimeUtc = DateTime.FromFileTimeUtc(info.CreationTime),
		LastAccessTimeUtc = DateTime.FromFileTimeUtc(info.LastAccessTime),
		LastWriteTimeUtc = DateTime.FromFileTimeUtc(info.LastWriteTime),
		ChangeTimeUtc = DateTime.FromFileTimeUtc(info.ChangeTime),
		Attributes = (FileAttributes)info.FileAttributes
	};

	public static implicit operator FileBasicInfo(EntryBasicInfo info) => new() {
		CreationTime = info.CreationTimeUtc.ToFileTimeUtc(),
		LastAccessTime = info.LastAccessTimeUtc.ToFileTimeUtc(),
		LastWriteTime = info.LastWriteTimeUtc.ToFileTimeUtc(),
		ChangeTime = info.ChangeTimeUtc.ToFileTimeUtc(),
		FileAttributes = (uint)info.Attributes
	};
}