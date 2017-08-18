using System;
using System.Reflection;
using Xamarin.Forms;

namespace Esri.ArcGISRuntime.Internal
{
	/// <summary>
	/// Provides members for registering BindableProperty instances
	/// </summary>
	internal class DependencyProperty
	{
		/// <summary>
		/// Registers a BindableProperty for the specified type
		/// </summary>
		internal static BindableProperty Register(string propertyName, Type returnType, Type declaringType, PropertyMetadata metadata)
		{
			BindableProperty prop = null;

			if (metadata != null)
			{
				prop = BindableProperty.Create(propertyName, returnType, declaringType,
					metadata.DefaultValue, BindingMode.OneWay, null, metadata.BindablePropertyChanged);
				metadata.Property = prop;
			}
			else
			{
				prop = BindableProperty.Create(propertyName, returnType, declaringType,
					GetDefaultValue(returnType), BindingMode.OneWay, null, null);
			}

			return prop;
		}

		/// <summary>
		/// Registers an attached BindableProperty for the specified type
		/// </summary>
		/// <returns></returns>
		internal static BindableProperty RegisterAttached(string propertyName, Type returnType, Type declaringType, PropertyMetadata metadata)
		{
			BindableProperty prop = null;

			if (metadata != null)
			{
				prop = BindableProperty.CreateAttached(propertyName, returnType, declaringType,
					metadata.DefaultValue, BindingMode.OneWay, null, metadata.BindablePropertyChanged);
				metadata.Property = prop;
			}
			else
			{
				prop = BindableProperty.CreateAttached(propertyName, returnType, declaringType,
					GetDefaultValue(returnType), BindingMode.OneWay, null, null);
			}

			return prop;
		}

		/// <summary>
		/// Retrieve the default value for the given type
		/// </summary>
		/// <param name="type"></param>
		private static object GetDefaultValue(Type type)
		{
#if NETFX_CORE
            var isValueType = type.GetTypeInfo().IsValueType;
#else
            var isValueType = type.IsValueType;
#endif
            if (isValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}
	}
}
