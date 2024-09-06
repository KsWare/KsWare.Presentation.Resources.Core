using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KsWare.Presentation.Resources.Core {

	public class FlatResourceDictionary : ResourceDictionaryEx {

		public override void EndInit() {
			var dic = new Dictionary<object, object>();
			Collect(this, dic);
			MergedDictionaries.Clear();
			Clear();
			foreach (var de in dic) Add(de.Key,de.Value);
			base.EndInit();
		}

		private void Collect(ResourceDictionary current, Dictionary<object, object> col) {
			foreach (var rd in current.MergedDictionaries) Collect(rd, col);
			foreach (DictionaryEntry de in current) col[de.Key] = de.Value;
		}

	}
}
