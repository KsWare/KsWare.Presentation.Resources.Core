using System;
using System.IO;
using System.Windows.Markup;

namespace KsWare.Presentation.Themes.Core {

	public class ThisFileNameExtension : MarkupExtension {

		public override object ProvideValue(IServiceProvider serviceProvider) {
			// Versuche den Dateinamen des XAML-Dokuments zu erhalten
			var service = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;

			if (service?.BaseUri != null) {
				return Path.GetFileName(service.BaseUri.LocalPath);
			}

			return null; // Oder eine alternative Rückgabe, falls kein Dateiname ermittelt werden kann
		}
	}
}
