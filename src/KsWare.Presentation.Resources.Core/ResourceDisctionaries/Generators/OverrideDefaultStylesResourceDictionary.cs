using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace KsWare.Presentation.Themes.Core {

	public class OverrideDefaultStylesResourceDictionary : ResourceDictionaryEx {

		private Dictionary<Type,bool> _lazyBasedOn = new Dictionary<Type, bool>();
		public string Prefix { get; set; }
		private bool _deferred = false;

		protected override void OnGettingValue(object key, ref object value, out bool canCache) {
			if (_deferred && value is Style style && key is Type t) {
				if (_lazyBasedOn.TryGetValue(t, out var isInitialized) && !isInitialized) {
					InitializeStyle(style);
					_lazyBasedOn[t] = true;
				}
			}
			base.OnGettingValue(key, ref value, out canCache);
		}

		private void InitializeStyle(Style style) {
			var baseStyleKey = $"{Prefix}{style.TargetType.Name}Style";
			var baseStyle = (Style)TryFindResource(baseStyleKey, this);
			style.BasedOn = baseStyle;
			Trace.Info($"initialize <Style TargetType=\"{style.TargetType.Name}\" BasedOn=\"{baseStyleKey}\"/>");
		}

		public override void EndInit() {
			AddControlStyles(InitialParent);
			base.EndInit();
		}

		protected override void OnAddParent(ResourceDictionary resourceDictionary) {
			base.OnAddParent(resourceDictionary);
		}

		private void AddControlStyles(ResourceDictionaryEx parent) {
			var controlTypes = typeof(Control).Assembly
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(Control)) && !t.IsAbstract);

			foreach (var controlType in controlTypes) {
				var baseStyleKey = $"Aero2Dark.{controlType.Name}Style";
//				var dic = new ResourceDictionary();
//				current.MergedDictionaries.Add(dic);
				var dic = this;

				if (_deferred) {
					if (TryFindKey(baseStyleKey, parent, dic) == null) continue;
					var style = new Style(controlType);
					dic[controlType] = style;
					_lazyBasedOn.Add(controlType, false);
					Debug.WriteLine($"create <Style TargetType=\"{controlType.Name}\" lazy.BasedOn=\"{baseStyleKey}\"/>");
				}
				else {
					var baseStyle = (Style) TryFindResource(baseStyleKey, parent);
					if (baseStyle == null) continue;
					dic[controlType] = new Style(controlType, baseStyle) {BasedOn = baseStyle};
					Debug.WriteLine($"create <Style TargetType=\"{controlType.Name}\" BasedOn=\"{baseStyleKey}\"/>");
				}
			}
		}
	}

}