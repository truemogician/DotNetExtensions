using System.IO;

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
}