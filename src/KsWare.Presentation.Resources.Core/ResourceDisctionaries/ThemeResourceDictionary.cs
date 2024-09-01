using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace KsWare.Presentation.Themes.Core {

	public class ThemeResourceDictionary : ResourceDictionaryEx {

		private ResourceDictionary _tempMerged;

		internal static ThemeResourceDictionary Current;

		/// <summary>
		/// Gets or sets the theme name.
		/// </summary>
		/// <value>The theme name.</value>
		public string ThemeName { get; set; }

		public new Uri Source {
			get => base.Source;
			set {
				base.Source = value;
				if (ThemeName == null) {
					ThemeName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(value.OriginalString.Split(new []{'\\','/'}).Last()));
				}
			}
		}

		public override void EndInit() {
			base.EndInit();
		}

		internal void AddTemporary(ResourceDictionary resourceDictionary) {
			if (_tempMerged == null) _tempMerged = new ResourceDictionary();
			_tempMerged.MergedDictionaries.Add(resourceDictionary);
		}

		internal void RemoveTemporary(ResourceDictionary resourceDictionary) {
			if (_tempMerged == null) return;
			_tempMerged.MergedDictionaries.Remove(resourceDictionary);
		}
	}

}
