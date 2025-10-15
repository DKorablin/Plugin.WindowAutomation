using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Plugin.WindowAutomation
{
	/// <summary>JSON serialization helper based on Newtonsoft.Json with optional strict type allow-list.</summary>
	/// <remarks>
	/// The legacy JavaScriptSerializer was replaced with Json.NET. For security, polymorphic (de)serialization
	/// is restricted via an explicit allow-list provided through the <c>allowedTypes</c> params argument.
	/// If no allowed types are supplied, polymorphic type name handling is disabled.
	/// </remarks>
	internal static class Serializer
	{
		/// <summary>
		/// Custom binder restricting (de)serialization only to explicitly allowed types when <see cref="TypeNameHandling"/> is enabled.
		/// Prevents unsafe type name resolution attacks by rejecting any type not in the provided list.
		/// </summary>
		private sealed class AllowedTypesSerializationBinder : ISerializationBinder
		{
			private readonly Type[] _allowedTypes;

			/// <summary>Create a new binder with the supplied allow-list.</summary>
			/// <param name="allowedTypes">Types that are permitted to participate in polymorphic (de)serialization.</param>
			public AllowedTypesSerializationBinder(params Type[] allowedTypes)
			{
				_ = allowedTypes ?? throw new ArgumentNullException(nameof(allowedTypes));
				_allowedTypes = (Type[])allowedTypes.Clone();
			}

			/// <inheritdoc />
			public Type BindToType(String assemblyName, String typeName)
			{
				// typeName is FullName when we emit without assembly name
				Type match = _allowedTypes.FirstOrDefault(t => t.FullName == typeName || t.AssemblyQualifiedName == typeName)
					?? throw new ArgumentException("Unknown or not allowed type: " + typeName, nameof(typeName));

				return match;
			}

			/// <inheritdoc />
			public void BindToName(Type serializedType, out String assemblyName, out String typeName)
			{
				if(this._allowedTypes.Contains(serializedType))
				{
					// We purposely omit assembly name for more compact JSON & matching BindToType logic
					assemblyName = null;
					typeName = serializedType.FullName;
					return;
				}

				throw new InvalidOperationException("Cannot serialize an object of type " + serializedType + ". Did you forget to add it to the allow list?");
			}
		}

		/// <summary>
		/// Build serializer settings. Enables <see cref="TypeNameHandling.Objects"/> only when an allow-list is supplied; otherwise leaves
		/// type name handling disabled (safer for simple DTOs with no polymorphism).
		/// </summary>
		/// <param name="allowedTypes">Optional list of allowed concrete types for polymorphic serialization.</param>
		private static JsonSerializerSettings CreateSettings(Type[] allowedTypes)
		{
			if(allowedTypes != null && allowedTypes.Length > 0)
			{
				return new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Objects,
					SerializationBinder = new AllowedTypesSerializationBinder(allowedTypes)
				};
			}

			// No polymorphic types required
			return new JsonSerializerSettings();
		}

		/// <summary>Deserialize a JSON string into an object.</summary>
		/// <typeparam name="T">Target object type.</typeparam>
		/// <param name="json">JSON input.</param>
		/// <param name="allowedTypes">Optional allow-list enabling safe polymorphic deserialization.</param>
		/// <returns>Deserialized object instance or default if input is null/empty.</returns>
		public static T JavaScriptDeserialize<T>(String json, params Type[] allowedTypes)
		{
			if(String.IsNullOrEmpty(json))
				return default;

			JsonSerializerSettings settings = CreateSettings(allowedTypes);
			return JsonConvert.DeserializeObject<T>(json, settings);
		}

		/// <summary>Serialize an object to JSON.</summary>
		/// <param name="item">Object to serialize.</param>
		/// <param name="allowedTypes">Optional allow-list enabling safe polymorphic serialization.</param>
		/// <returns>JSON string or null if <paramref name="item"/> is null.</returns>
		public static String JavaScriptSerialize(Object item, params Type[] allowedTypes)
		{
			if(item == null)
				return null;

			JsonSerializerSettings settings = CreateSettings(allowedTypes);
			return JsonConvert.SerializeObject(item, settings);
		}
	}
}