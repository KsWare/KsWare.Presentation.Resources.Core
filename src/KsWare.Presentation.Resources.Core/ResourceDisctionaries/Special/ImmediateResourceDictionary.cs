using System;
using System.Windows;

namespace KsWare.Presentation.Resources.Core {

	public class ImmediateResourceDictionary : ResourceDictionaryEx {

		private Uri _source;

		public new Uri Source {
			get => _source;
			set {
				if (value == null) return;
				_source = value;
				LoadResourceDictionary();
			}
		}

		private void LoadResourceDictionary() {
			var loadedDictionary = new ResourceDictionary {Source = base.Source};
			foreach (var key in loadedDictionary.Keys) {
				this[key] = loadedDictionary[key];
			}
			foreach (var dictionary in loadedDictionary.MergedDictionaries) {
				MergedDictionaries.Add(dictionary);
			}
		}
	}

}
