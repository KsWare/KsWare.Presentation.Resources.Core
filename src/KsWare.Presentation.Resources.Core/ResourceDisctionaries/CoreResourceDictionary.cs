using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace KsWare.Presentation.Resources.Core {

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
		/// <remarks>Uses the same logic as in <see cref="FrameworkElement.TryFindResource">FrameworkElement.TryFindResource</see>, <see cref="Application.TryFindResource">Application.TryFindResource</see>.</remarks>
		/// <seealso cref="FrameworkElement.TryFindResource"/>
		/// <seealso cref="Application.TryFindResource"/>
		public object? TryFindResource(object resourceKey) => TryFindResource(resourceKey, this);

		protected static object? TryFindResource(object key, ResourceDictionary resourceDictionary, bool skipRdWithSource = false, bool skipCurrentKeys = false) {
			if (!skipCurrentKeys && resourceDictionary.Contains(key)) return resourceDictionary[key];
			foreach (var mergedDict in resourceDictionary.MergedDictionaries.Reverse()) {
				if(skipRdWithSource && mergedDict.Source!=null) continue;
				var result = TryFindResource(key, mergedDict);
				if (result!=null) return result;
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