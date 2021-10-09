using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TrueMogician.Exceptions;
using TrueMogician.Extensions.Enumerable;

namespace TrueMogician.Extensions.Reflection {
	public static class ReflectionExtensions {
		#nullable enable
		public static object? GetValue(this MemberInfo info, object obj)
			=> info switch {
				FieldInfo field       => field.GetValue(obj),
				PropertyInfo property => property.GetValue(obj),
				_                     => throw new MemberTypeException(info, MemberTypes.Property | MemberTypes.Field)
			};
		#nullable disable

		public static void SetValue(this MemberInfo info, object obj, object value) {
			switch (info) {
				case FieldInfo field:
					field.SetValue(obj, value);
					break;
				case PropertyInfo property:
					property.SetValue(obj, value);
					break;
				default: throw new MemberTypeException(info, MemberTypes.Property | MemberTypes.Field);
			}
		}

		public static void SetValueWithConversion(this MemberInfo member, object obj, object value) {
			var type = member.GetValueType();
			if (type.IsInstanceOfType(value) || value is null)
				member.SetValue(obj, value);
			else if (type.Implements(typeof(IConvertible)) && value.GetType().Implements(typeof(IConvertible)))
				member.SetValue(obj, Convert.ChangeType(value, type));
			else
				throw new InterfaceNotImplementedException(typeof(IConvertible));
		}

		public static Type GetValueType(this MemberInfo info)
			=> info switch {
				FieldInfo field             => field.FieldType,
				PropertyInfo property       => property.PropertyType,
				MethodInfo method           => method.ReturnType,
				ConstructorInfo constructor => constructor.DeclaringType,
				EventInfo @event            => @event.EventHandlerType,
				_                           => info.ReflectedType
			};

		/// <summary>
		/// </summary>
		/// <param name="type"></param>
		/// <param name="genericTypeDefinition">
		///     The generic interface <paramref name="type" /> is required to implement. Default is
		///     <see cref="IEnumerable{T}" />
		/// </param>
		/// <returns></returns>
		public static Type GetItemType(this Type type, Type genericTypeDefinition = null) {
			genericTypeDefinition ??= typeof(IEnumerable<>);
			if (!genericTypeDefinition.IsGenericTypeDefinition)
				throw new TypeException(genericTypeDefinition, "Required to be a generic type definition");
			if (!type.Implements(genericTypeDefinition))
				return null;
			return type.HasElementType ? type.GetElementType() : type.GetGenericInterfaceArguments(genericTypeDefinition).Single();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Implements(this Type type, Type interfaceType)
			=> (interfaceType.IsGenericTypeDefinition
				? type.GetGenericInterface(interfaceType)
				: type.GetInterface(interfaceType.Name)) is not null;

		public static Type[] GetGenericInterfaces(this Type type, Type genericTypeDefinition) {
			if (!genericTypeDefinition.IsGenericTypeDefinition)
				throw new TypeException(genericTypeDefinition, "Required to be a generic type definition");
			return type.GetInterfaces()
				.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeDefinition)
				.ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type GetGenericInterface(this Type type, Type genericTypeDefinition) => type.GetGenericInterfaces(genericTypeDefinition).SingleOrDefault();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type[] GetGenericInterfaceArguments(this Type type, Type genericTypeDefinition) => type.GetGenericInterface(genericTypeDefinition)?.GetGenericArguments();

		public static Type[] GetGenericTypes(this Type type, Type genericTypeDefinition) {
			if (!genericTypeDefinition.IsGenericTypeDefinition)
				throw new TypeException(genericTypeDefinition, "Required to be a generic type definition");
			if (genericTypeDefinition.IsInterface)
				return type.GetGenericInterfaces(genericTypeDefinition);
			do {
				var cur = type?.IsGenericType == true ? type.GetGenericTypeDefinition() : type;
				if (genericTypeDefinition == cur)
					return new[] {type};
				type = type?.BaseType;
			} while (type != null && type != typeof(object));
			return Array.Empty<Type>();
		}

		#nullable enable
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type? GetGenericType(this Type type, Type genericTypeDefinition) => type.GetGenericTypes(genericTypeDefinition).SingleOrDefault();
		#nullable disable

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAssignableToGeneric(this Type type, Type genericType) => type.GetGenericTypes(genericType).Length > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsIndexer(this PropertyInfo property) => property.GetIndexParameters().Length > 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PropertyInfo[] GetIndexers(this Type type) => type.GetProperties().Where(IsIndexer).AsArray();

		public static PropertyInfo GetIndexer(this Type type, params Type[] parameterTypes)
			=> type.GetProperties()
				.SingleOrDefault(
					p => p.GetIndexParameters() is var args &&
						args.Length == parameterTypes.Length &&
						parameterTypes.Select((t, index) => t == args[index].ParameterType).All(x => x)
				);

		public static MemberInfo[] GetMembersWithAllAttributes(this Type type, params Type[] attributeTypes) => type.GetMembers().Where(member => attributeTypes.All(member.IsDefined)).ToArray();

		public static MemberInfo[] GetMembersWithAnyAttributes(this Type type, params Type[] attributeTypes) => type.GetMembers().Where(member => attributeTypes.Any(member.IsDefined)).ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInfo[] GetMembersWithAttribute<T>(this Type type) where T : Attribute => type.GetMembersWithAllAttributes(typeof(T));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInfo GetMemberWithAllAttributes(this Type type, params Type[] attributeTypes) => type.GetMembersWithAllAttributes(attributeTypes).SingleOrDefault();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInfo GetMemberWithAnyAttributes(this Type type, params Type[] attributeTypes) => type.GetMembersWithAnyAttributes(attributeTypes).SingleOrDefault();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInfo GetMemberWithAttribute<T>(this Type type) where T : Attribute => type.GetMembersWithAttribute<T>().SingleOrDefault();

		public static (MemberInfo Member, Attribute[] Attributes)[] GetMembersAndAttributes(this Type type, params Type[] attributeTypes) => type.GetMembers().Where(member => attributeTypes.All(member.IsDefined)).Select(member => (member, attributeTypes.Select(member.GetCustomAttribute).ToArray())).ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (MemberInfo Member, T Attribute)[] GetMembersAndAttribute<T>(this Type type) where T : Attribute => type.GetMembersAndAttributes(typeof(T)).Select(result => (result.Member, result.Attributes.Single() as T)).ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (MemberInfo Member, Attribute[] Attributes) GetMemberAndAttributes(this Type type, params Type[] attributeTypes) => type.GetMembersAndAttributes(attributeTypes).SingleOrDefault();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static (MemberInfo Member, T Attribute) GetMemberAndAttribute<T>(this Type type) where T : Attribute => type.GetMembersAndAttribute<T>().SingleOrDefault();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type GetMemberType(this Type type, string name) => type.GetMember(name).SingleOrDefault()?.GetValueType();

		public static MemberInfo[] GetMembers(this Type type, MemberTypes memberTypes) {
			var result = new List<MemberInfo>();
			if (memberTypes.HasFlag(MemberTypes.Property))
				result.AddRange(type.GetProperties());
			if (memberTypes.HasFlag(MemberTypes.Field))
				result.AddRange(type.GetFields());
			if (memberTypes.HasFlag(MemberTypes.Constructor))
				result.AddRange(type.GetConstructors());
			if (memberTypes.HasFlag(MemberTypes.Event))
				result.AddRange(type.GetEvents());
			if (memberTypes.HasFlag(MemberTypes.Method))
				result.AddRange(type.GetMethods());
			if (memberTypes.HasFlag(MemberTypes.NestedType))
				result.AddRange(type.GetNestedTypes());
			return result.ToArray();
		}

		public static MemberInfo GetMember(this Type type, string name, MemberTypes memberTypes) {
			var members = type.GetMember(name);
			return members.SingleOrDefault(member => memberTypes.HasFlag(member.MemberType));
		}

		private static T GetMostDerivedMember<T>(Type type, string name, Func<Type, IEnumerable<T>> getMembers) where T : MemberInfo {
			var members = getMembers(type).Where(p => p.Name == name).ToArray();
			if (members.Length <= 1)
				return members.Length == 0 ? null : members[0];
			return members.OrderBy(p => p.DeclaringType, new TypeInheritanceComparer()).FirstOrDefault();
		}

		private static T[] GetMostDerivedMembers<T>(Type type, Func<Type, IEnumerable<T>> getMembers) where T : MemberInfo {
			var comparer = new TypeInheritanceComparer();
			return getMembers(type)
				.GroupBy(p => p.Name)
				.Select(group => group.OrderBy(p => p.DeclaringType, comparer).FirstOrDefault())
				.ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInfo GetMostDerivedMember(this Type type, string name) => GetMostDerivedMember(type, name, t => t.GetMembers());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInfo[] GetMostDerivedMembers(this Type type) => GetMostDerivedMembers(type, t => t.GetMembers());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PropertyInfo GetMostDerivedProperty(this Type type, string name) => GetMostDerivedMember(type, name, t => t.GetProperties());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PropertyInfo[] GetMostDerivedProperties(this Type type) => GetMostDerivedMembers(type, t => t.GetProperties());

		public static TResult[] GetAttributeValues<TAttribute, TResult>(this Type type, Func<TAttribute, TResult> selector) where TAttribute : Attribute {
			var properties = type.GetProperties();
			return properties.Select(p => selector(p.GetCustomAttribute<TAttribute>())).ToArray();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object Construct(this Type type, params object[] parameters) => Activator.CreateInstance(type, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object Invoke(this MethodInfo method, object obj, params object[] parameters) => method.Invoke(obj, parameters);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static BindingFlags GetBindingFlags(this MemberInfo member) => (BindingFlags)member.GetType().GetProperty("BindingFlags", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(member)!;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FieldInfo GetEnumMember(this Enum enumValue) => enumValue.GetType().GetField(enumValue.ToString());
		#nullable disable
	}

	public class TypeInheritanceComparer : IComparer<Type> {
		public int Compare(Type x, Type y) {
			if (x is null)
				return y is null ? 0 : -1;
			if (y is null)
				return 1;
			if (x.IsSubclassOf(y))
				return -1;
			return y.IsSubclassOf(x) ? 1 : 0;
		}
	}
}