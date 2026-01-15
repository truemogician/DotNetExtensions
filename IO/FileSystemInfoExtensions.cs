using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TrueMogician.Extensions.IO;

public static class FileSystemInfoExtensions {
	/// <summary>
	///     Determines whether the specified file or directory exists.
	/// </summary>
	/// <remarks>
	///     This method returns <see langword="true"/> if the caller does not have sufficient permissions to read the specified
	///     file or directory, but the file or directory does exist. The method does not check whether the caller has
	///     permission to access the file or directory beyond its existence.
	/// </remarks>
	/// <param name="path">
	///     The path to the file or directory to check. The path is not case-sensitive. This parameter can refer to either a
	///     file or a directory.
	/// </param>
	/// <returns><see langword="true"/> if the specified file or directory exists; otherwise, <see langword="false"/>.</returns>
	public static bool Exists(string path) {
		try {
			File.GetAttributes(path);
			return true;
		}
		catch (UnauthorizedAccessException) {
			return true;
		}
		catch (FileNotFoundException) {
			return false;
		}
		catch (DirectoryNotFoundException) {
			return false;
		}
	}

	extension(FileSystemInfo self) {
#if !NETSTANDARD2_0 && !NETFRAMEWORK
		public string RelativeTo(DirectoryInfo @base)
			=> Path.GetRelativePath(@base.FullName, self.FullName);

		public string RelativeTo(string @base)
			=> Path.GetRelativePath(Path.GetFullPath(@base), self.FullName);
#endif
	}

	extension(FileInfo self) {
		public string NameWithoutExtension => Path.GetFileNameWithoutExtension(self.Name);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rename(string name)
			=> self.MoveTo(self.Directory!.ChildPath(name));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RenameWithoutExtension(string name, string? newExt = null) {
			string newName = newExt is null ? name + self.Extension : name + newExt;
			self.MoveTo(self.Directory!.ChildPath(newName));
		}
	}

	extension(DirectoryInfo self) {
		public long Size =>
#if NETSTANDARD2_0 || NETFRAMEWORK
			self.EnumerateFiles("*", SearchOption.AllDirectories)
#else
			self.EnumerateFiles("*", new EnumerationOptions {
				AttributesToSkip = 0,
				RecurseSubdirectories = true
			})
#endif
				.Sum(f => f.Length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ChildPath(string subPath) => Path.Combine(self.FullName, subPath);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ResolveRelative(string relativePath) => Path.GetFullPath(Path.Combine(self.FullName, relativePath));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rename(string baseName)
			=> self.MoveTo(self.Parent!.ChildPath(baseName));
	}
}