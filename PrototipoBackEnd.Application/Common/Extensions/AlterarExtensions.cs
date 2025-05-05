using System.Reflection;

namespace PrototipoBackEnd.Application.Common.Extensions
{
	public static class AlterarExtensions
	{
		public static void ApplyIfChanged<T>(this T target, T source)
		{
			var type = typeof(T);
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var prop in properties)
			{
				// Ignora propriedades que não podem ser escritas
				if (!prop.CanWrite || !prop.CanRead) continue;

				var sourceValue = prop.GetValue(source);
				var targetValue = prop.GetValue(target);

				if (sourceValue != null && !Equals(sourceValue, targetValue))
				{
					prop.SetValue(target, sourceValue);
				}
			}
		}

	}
}
