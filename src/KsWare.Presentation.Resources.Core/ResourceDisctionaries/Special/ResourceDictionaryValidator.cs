using System;
using System.ComponentModel;
using System.Windows;

namespace KsWare.Presentation.Resources.Core {

	public class ResourceDictionaryWithInitializer : ResourceDictionary, ISupportInitialize {

		public ResourceDictionaryWithInitializer() { }

		public event EventHandler OnBeginInit;
		public event EventHandler OnEndInit;

		public new void BeginInit() {
			base.BeginInit();
			OnBeginInit?.Invoke(this, EventArgs.Empty);
		}

		public new void EndInit() {
			base.EndInit();
			OnEndInit?.Invoke(this, EventArgs.Empty);
		}

	}

}
