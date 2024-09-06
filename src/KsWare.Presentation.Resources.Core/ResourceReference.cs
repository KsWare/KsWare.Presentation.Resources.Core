using System.Collections;
using System.ComponentModel;
using System.Windows;

namespace KsWare.Presentation.Resources.Core {

	public class ResourceReference : DependencyObject, ISupportInitialize {

		public object ReferenceKey { get; set; }

		public ResourceReference() { }

		public ResourceReference(object referenceKey) {
			ReferenceKey = referenceKey;
		}

		void ISupportInitialize.BeginInit() { }

		void ISupportInitialize.EndInit() {
			ReplaceSelf();
		}

		private void ReplaceSelf() {
			if (TryFindResourceByValue(Application.Current.Resources, this, out var key, out var dictionary )) {
				if (TryFindResource(Application.Current.Resources, ReferenceKey, out var v, out _)) {
					dictionary[key] = v;
				}
			}
//			if (TryFindResource(Application.Current.Resources, KeyExtension.LastUsedKey, out var key2, out var dictionary2 )) {
//				
//			}
		}

		bool TryFindResource(ResourceDictionary resources, object key, out object value, out ResourceDictionary resourceDictionary ) {
			var foundValue = (object) null;
			var foundResourceDictionary = (ResourceDictionary) null;
			
			foreach (var mergedDict in resources.MergedDictionaries) {
				if(TryFindResource(mergedDict, key, out var v, out var d)) {
					foundValue = v;
					foundResourceDictionary = d;
				}
			}

			if (resources.Contains(key)) {
				resourceDictionary = resources;
				value = resources[key];
				return true;
			}

			value = foundValue;
			resourceDictionary = foundResourceDictionary;
			return foundResourceDictionary!=null;
		}

		bool TryFindResourceByValue(ResourceDictionary resources, object value, out object key, out ResourceDictionary resourceDictionary ) {
			var foundKey = (object) null;
			var foundResourceDictionary = (ResourceDictionary) null;
			
			foreach (var mergedDict in resources.MergedDictionaries) {
				if(TryFindResourceByValue(mergedDict, value, out var k, out var d)) {
					foundKey = k;
					foundResourceDictionary = d;
				}
			}

			foreach (DictionaryEntry entry in resources) {
				if (Equals(entry.Value, value)) {
					key = entry.Key;
					resourceDictionary = resources;
					return true;
				}
			}

			key = foundKey;
			resourceDictionary = foundResourceDictionary;
			return foundResourceDictionary!=null;
		}

	}

}
