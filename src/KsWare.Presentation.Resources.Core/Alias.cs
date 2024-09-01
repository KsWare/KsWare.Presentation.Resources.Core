using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using KsWare.Presentation.Core.Utils;
using DispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace KsWare.Presentation.Themes.Core {

	public class Alias : DependencyObject, ISupportInitialize {

		public static readonly DependencyProperty ResourceProperty = DependencyProperty.Register(
			nameof(Resource), typeof(object), typeof(Alias), new FrameworkPropertyMetadata(default(object),(d,e)=>((Alias)d).OnResourceChanged(e)));

		public static readonly DependencyProperty ResourceKeyProperty = DependencyProperty.Register(
			nameof(ResourceKey), typeof(object), typeof(Alias), new FrameworkPropertyMetadata(default(object),(d,e)=>((Alias)d).OnResourceKeyChanged(e)));

		public object ResourceKey { get => (object) GetValue(ResourceKeyProperty); set => SetValue(ResourceKeyProperty, value);}
		
		public object Resource { get => (object) GetValue(ResourceProperty); set => SetValue(ResourceProperty, value);}

		public Alias() {
			Console.WriteLine("Alias.ctor");
			Debug.WriteLine("Alias.ctor");
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(PostInit));
		}

		private void PostInit() {
			Console.WriteLine("Alias.PostInit");
			Debug.WriteLine("Alias.PostInit");
		}

		private void OnResourceKeyChanged(DependencyPropertyChangedEventArgs e) {
			var newValue = (object) e.NewValue;
			var oldValue = (object) e.OldValue;
		}

		private void OnResourceChanged(DependencyPropertyChangedEventArgs e) {
			var newValue = (object) e.NewValue;
			var oldValue = (object) e.OldValue;
		}

		void ISupportInitialize.BeginInit() { }

		void ISupportInitialize.EndInit() {
			Console.WriteLine("Alias.EndInit");
			Debug.WriteLine("Alias.EndInit");
			ReplaceSelf();
		}

		private void ReplaceSelf() {
			var thisResource = ResourceHelper.FindResourceByValue(this);
			if(thisResource==null) return;
			
			if (Resource != null) {
				thisResource.SourceDictionary[thisResource.Key] = Resource;
				return;
			}
			if (ResourceKey != null) {
				var resource = ResourceHelper.FindResource(ResourceKey);
				if(resource==null) return;

				thisResource.SourceDictionary[thisResource.Key] = resource.Value;
				return;
			}
		}

	}

}
