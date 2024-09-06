using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace KsWare.Presentation.Resources.Core {

	[MarkupExtensionReturnType(typeof(ResourceDictionary))]
	public class ResourceDictionaryReferenceExtension : StaticResourceExtension {
		
		public override object? ProvideValue(IServiceProvider serviceProvider) {
			try {
				return base.ProvideValue(serviceProvider);
			}
			catch {
				return new ResourceDictionary();
			}
		}
	}
}
