using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TrueMogician.Extensions.IO;

public static class FileSystemInfoExtensions {
	extension(FileSystemInfo self) {
		public string NameWithoutExtension => Path.GetFileNameWithoutExtension(self.Name);

#if !NETSTANDARD2_0
		public string RelativeTo(DirectoryInfo @base)
			=> Path.GetRelativePath(@base.FullName, self.FullName);

		public string RelativeTo(string @base)
			=> Path.GetRelativePath(Path.GetFullPath(@base), self.FullName);
#endif
	}

	extension(FileInfo self) {
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
#if NETSTANDARD2_0
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