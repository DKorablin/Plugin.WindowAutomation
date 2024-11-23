using System;
using System.Linq;
using System.Web.Script.Serialization;

namespace Plugin.WindowAutomation
{
	/// <summary>Сериализация</summary>
	internal static class Serializer
	{
		class CustomTypeResolver : JavaScriptTypeResolver
		{
			private readonly Type[] _allowedTypes;

			public CustomTypeResolver(params Type[] allowedTypes)
			{
				_ = allowedTypes ?? throw new ArgumentNullException(nameof(allowedTypes));

				this._allowedTypes = (Type[])allowedTypes.Clone();
			}

			public override Type ResolveType(String id)
			{
				foreach(Type allowedType in this._allowedTypes)
					if(allowedType.FullName == id)
						return allowedType;

				throw new ArgumentException("Unknown type: " + id, "id");
			}

			public override String ResolveTypeId(Type type)
			{
				if(_allowedTypes.Contains(type))
					return type.FullName;

				throw new InvalidOperationException("Cannot serialize an object of type " + type + ". Did you forget to add it to the allow list?");
			}
		}

		/// <summary>Десериализовать строку в объект</summary>
		/// <typeparam name="T">Тип объекта</typeparam>
		/// <param name="json">Строка в формате JSON</param>
		/// <returns>Десериализованный объект</returns>
		public static T JavaScriptDeserialize<T>(String json, params Type[] allowedTypes)
		{
			if(String.IsNullOrEmpty(json))
				return default;

			JavaScriptSerializer serializer = new JavaScriptSerializer(new CustomTypeResolver(allowedTypes));
			return serializer.Deserialize<T>(json);
		}

		/// <summary>Сериализовать объект</summary>
		/// <param name="item">Объект для сериализации</param>
		/// <returns>Строка в формате JSON</returns>
		public static String JavaScriptSerialize(Object item, params Type[] allowedTypes)
		{
			if(item == null)
				return null;

			JavaScriptSerializer serializer = new JavaScriptSerializer(new CustomTypeResolver(allowedTypes));
			return serializer.Serialize(item);
		}
	}
}