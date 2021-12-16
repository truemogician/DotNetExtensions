using System;

namespace TrueMogician.Exceptions {
	public class FileSystemEntityException : ExceptionWithDefaultMessage {
		public FileSystemEntityException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		public FileSystemEntityException(string path, string? message = null, Exception? innerException = null) : base(message, innerException) => Path = path;

		public string? Path { get; init; }
	}

	public class FileSystemEntityNotFoundException : FileSystemEntityException {
		public FileSystemEntityNotFoundException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		public FileSystemEntityNotFoundException(string path, string? message = null, Exception? innerException = null) : base(path, message, innerException) { }

		protected override string? DefaultMessage => $"File or directory {(Path is null ? "" : $"\"{Path}\" ")}not found";
	}

	public class FileSystemEntityExistedException : FileSystemEntityException {
		public FileSystemEntityExistedException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		public FileSystemEntityExistedException(string path, string? message = null, Exception? innerException = null) : base(path, message, innerException) { }

		protected override string? DefaultMessage => $"File or directory {(Path is null ? "" : $"\"{Path}\" ")}already existed";
	}

	public class FileExistedException : FileSystemEntityException {
		public FileExistedException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		public FileExistedException(string path, string? message = null, Exception? innerException = null) : base(path, message, innerException) { }

		protected override string? DefaultMessage => $"File {(Path is null ? "" : $"\"{Path}\" ")}already existed";
	}

	public class DirectoryExistedException : FileSystemEntityException {
		public DirectoryExistedException(string? message = null, Exception? innerException = null) : base(message, innerException) { }

		public DirectoryExistedException(string path, string? message = null, Exception? innerException = null) : base(path, message, innerException) { }

		protected override string? DefaultMessage => $"Directory {(Path is null ? "" : $"\"{Path}\" ")}already existed";
	}
}