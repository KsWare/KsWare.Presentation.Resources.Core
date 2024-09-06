using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace KsWare.Presentation.Resources.Core {

	public class OverrideDefaultStylesResourceDictionary : ResourceDictionaryEx {

		private Dictionary<Type,bool> _lazyBasedOn = new Dictionary<Type, bool>();

		/// <summary>
		/// Base style key pattern. The star-char (*) will be replaced by type name.
		/// </summary>
		/// <example><c>MyTheme.*Style</c></example>
		public string KeyPattern { get; set; } = "Theme.*Style";

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
			var baseStyleKey = KeyPattern.Replace("*", style.TargetType.Name);
			var baseStyle = (Style)TryFindResource(baseStyleKey, LogicalParent);
			style.BasedOn = baseStyle;
			Trace.Info($"initialize <Style TargetType=\"{style.TargetType.Name}\" BasedOn=\"{baseStyleKey}\"/>");
		}

		public override void EndInit() {
			AddControlStyles(InitialParent);
			base.EndInit();
		}

		private void AddControlStyles(ResourceDictionaryEx parent) {
			var controlTypes = typeof(FrameworkElement).Assembly
				.GetTypes()
				.Where(t => t.IsSubclassOf(typeof(FrameworkElement)) && !t.IsAbstract);
			foreach (var controlType in controlTypes) {
				var baseStyleKey = KeyPattern.Replace("*", controlType.Name);
				var dic = this;

				if (_deferred) {
					if (TryFindKey(baseStyleKey, parent, dic) == null) continue;
					var style = new Style(controlType);
					dic[controlType] = style;
					_lazyBasedOn.Add(controlType, false);
					Debug.WriteLine($"create <Style TargetType=\"{controlType.Name}\" deferred.BasedOn=\"{baseStyleKey}\"/>");
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