using System;
using System.Windows.Markup;

namespace KsWare.Presentation.Themes.Core {

	public class KeyExtension : StaticExtension{

		public static object LastKey;

		private readonly object _key;

		public KeyExtension(object key) {
			LastKey = key;
			_key = key;
		}

		public override object ProvideValue(IServiceProvider serviceProvider) {
			return _key;
		}

	}

}
