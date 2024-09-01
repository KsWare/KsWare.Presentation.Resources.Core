using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace KsWare.Presentation.Themes.Core {

	/// <summary>
	/// Base class for all extended resource dictionaries
	/// </summary>
	public abstract class CoreResourceDictionary : ResourceDictionary, ISupportInitialize {

		/// <inheritdoc cref="ResourceDictionary.Source"/>>
		public new virtual Uri Source {
			get => base.Source;
			set => base.Source = value;
		}

		/// <inheritdoc cref="ResourceDictionary.BeginInit"/>>
		public new virtual void BeginInit() {
			base.BeginInit();
		}

		/// <inheritdoc cref="ResourceDictionary.EndInit"/>>
		public new virtual void EndInit() {
			base.EndInit();
		}

		/// <summary>
		/// Searches for a resource with the specified key, and returns that resource if found. 
		/// </summary>
		/// <param name="resourceKey">The key identifier of the resource to be found.</param>
		/// <returns>The found resource, or null if no resource with the provided key is found.</returns>
		public object? TryFindResource(object resourceKey) => TryFindResource(resourceKey, this);

		protected static object? TryFindResource(object key, ResourceDictionary resourceDictionary) {
			if (resourceDictionary.Contains(key)) return resourceDictionary[key];
			foreach (var mergedDict in resourceDictionary.MergedDictionaries.Reverse()) {
				var result = TryFindResource(key, mergedDict);
				if (result!=null) return true;
			}
			return null;
		}

		protected static ResourceDictionary? TryFindKey(object key, ResourceDictionary resourceDictionary, ResourceDictionary? stop) {
			if (resourceDictionary.Contains(key)) return resourceDictionary;

			var startIndex = resourceDictionary.MergedDictionaries.Count - 1;
			if (stop != null) {
				var i = resourceDictionary.MergedDictionaries.IndexOf(stop);
				if (i >= 0) startIndex = i - 1;
			}

			for (int i = startIndex; i >= 0; i--) {
				var mergedDict = resourceDictionary.MergedDictionaries[i];
				var result = TryFindKey(key, mergedDict, stop);
				if (result!=null) return mergedDict;
			}

			return null;
		}
	}

}