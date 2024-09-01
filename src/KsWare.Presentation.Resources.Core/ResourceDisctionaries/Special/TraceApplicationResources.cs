using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace KsWare.Presentation.Themes.Core {

	public class TraceApplicationResources : ResourceDictionary, ISupportInitialize {

		public TraceApplicationResources() {
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send, new Action(PostInit));
		}

		public bool TraceKeys { get; set; } = true;
		public bool TraceMergedDictionaries { get; set; } = true;
		public bool TraceRecursive { get; set; } = true;
		public bool UsePostInit { get; set; }

		void Enumerate(ResourceDictionary resourceDictionary, int level = 0) {
			var ind = new string(' ', level * 2);
			Debug.WriteLineIf(TraceMergedDictionaries, $"{ind}<{GetDisplayInfo(resourceDictionary)}>");
			if (TraceRecursive) {
				foreach (var mergedDict in resourceDictionary.MergedDictionaries) {
					Enumerate(mergedDict, level+1);
				}
			}
			foreach (var key in resourceDictionary.Keys) {
				Debug.WriteLineIf(TraceKeys, $"{ind}  {key}");
			}
		}

		private string GetDisplayInfo(ResourceDictionary r) {
			var sSource = r.Source != null ? $" Source:{r.Source}" : "";
			switch (r) {
				case ResourceDictionaryEx re: {
					var sName = re.Name != null ? $" Name:{re.Name}" : "";
					var sLocation = re.Location != null ? $" Location:{re.Location}" : "";
					return $"{r.GetType().Name}({re.InstanceId}){sName}{sSource}{sLocation}";
				}
				default: return $"{r.GetType().Name}{sSource}";
			}
		}

		public new void BeginInit() {
			
		}

		public new void EndInit() {
			if(UsePostInit) return;
			Debug.WriteLine($"TraceApplicationResources:");
			Enumerate(Application.Current.Resources);
		}

		public void PostInit() {
			if(!UsePostInit) return;
			Debug.WriteLine($"TraceApplicationResources:");
			Enumerate(Application.Current.Resources);
		}

	}
}
