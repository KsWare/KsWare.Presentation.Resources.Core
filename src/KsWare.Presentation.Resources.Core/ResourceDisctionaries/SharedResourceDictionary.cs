using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace KsWare.Presentation.Themes.Core {

	/// <summary>
	/// A ResourceDictionary that caches and reuses resources.
	/// </summary>
	public class SharedResourceDictionary : ResourceDictionary {

		private static readonly Dictionary<Uri, WeakReference<ResourceDictionary>> SharedCache = new Dictionary<Uri, WeakReference<ResourceDictionary>>();
		private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
		
		/// <summary>Gets or sets the shared uniform resource identifier (URI) to load resources from.</summary>
		/// <returns>The source location of an external resource dictionary.</returns>
		public new Uri Source {
			get => IsInDesignMode ? base.Source : null;
			set => SharedSource = value;
		}

		/// <summary>Gets or sets the shared uniform resource identifier (URI) to load resources from.</summary>
		/// <returns>The source location of an external resource dictionary.</returns>
		public Uri SharedSource {
			get =>  IsInDesignMode ? base.Source : MergedDictionaries.FirstOrDefault()?.Source;
			set { 
				if (IsInDesignMode) { base.Source = value; return; }
				if (!SharedCache.TryGetValue(value, out var weakRef) || !weakRef.TryGetTarget(out var resourceDictionary)) {
					resourceDictionary = new ResourceDictionary { Source = value };
					SharedCache[value] = new WeakReference<ResourceDictionary>(resourceDictionary, false);
				}
				MergedDictionaries.Add(resourceDictionary);
			}
		}

	}
}