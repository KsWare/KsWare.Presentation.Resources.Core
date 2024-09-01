using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KsWare.Presentation.Themes.Core {

	public class InstantResourceDictionary : ResourceDictionaryEx  {
		
		public override void EndInit() {
			base.EndInit();
			Application.Current.Resources.MergedDictionaries.Add(this);
		}

		protected override void PostInit() {
			base.PostInit();
			Application.Current.Resources.Remove(this);
		}

	}
}
