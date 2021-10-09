using System;

namespace TrueMogician.Exceptions {
	public class EnumException : TypeException {
		public EnumException(string message = null, Exception innerException = null) : base(message, innerException) { }

		public EnumException(Type enumType, string message = null, Exception innerException = null) : base(enumType, message, innerException) {
			if (!typeof(Enum).IsAssignableFrom(enumType))
				throw new InvariantTypeException(typeof(Enum), enumType);
		}
	}

	public class EnumValueOutOfRangeException : EnumException {
		public EnumValueOutOfRangeException(string message = null, Exception innerException = null) : base(message, innerException) { }

		public EnumValueOutOfRangeException(Type enumType, object enumValue = null, string message = null, Exception innerException = null) : base(enumType, message, innerException) => this[nameof(EnumValue)] = enumValue;

		public object EnumValue => Get<object>(nameof(EnumValue));

		protected override string DefaultMessage => $"{EnumValue} is out of the required range of {Type.FullName}";
	}
}