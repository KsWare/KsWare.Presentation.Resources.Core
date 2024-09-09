using System;
using System.Diagnostics;
using System.Text;

namespace KsWare.Presentation.Resources.Core.Internal {

	internal class RdTrace {

		private ResourceDictionaryEx? _caller;

		public bool IsEnabled { get; private set; }
		public bool ToDebugOutput { get; set; } = true;
		public bool ToConsoleOutput { get; set; }

		public void SetIsEnabled(bool value, ResourceDictionaryEx caller) {
			if (value) {
				if(_caller != null) return;
				_caller = caller;
				IsEnabled = true;
			}
			else {
				if(_caller!=caller) return;
				IsEnabled = false;
			}
		}

		[Conditional("DEBUG"), Conditional("TRACE")]
		public void InfoIf(bool condition, string msg) {
			if(!condition || !IsEnabled) return;
			if(ToDebugOutput) Debug.WriteLine(msg);
			if(ToConsoleOutput) Console.WriteLine(msg);
		}

		[Conditional("DEBUG"), Conditional("TRACE")]
		public void Info(string msg) => InfoIf(true, msg);

//		public static void Info(string message, params object[] optionalParams) {
//			var formattedMessage = new StringBuilder(message);
//
//			for (var i = 0; i < optionalParams.Length; i += 2) {
//				var name = optionalParams[i] as string;
//				var value = optionalParams[i + 1];
//
//				if (value != null) {
//					formattedMessage.Append($" {name}:{value}");
//				}
//			}
//			Debug.WriteLine(formattedMessage.ToString());
//		}

		[Conditional("DEBUG"),Conditional("TRACE")]
		public void Info(string msg, params (string Name, object? Value)[] optionalParams) {
			var formattedMessage = new StringBuilder(msg);

			foreach (var param in optionalParams) {
				if (param.Value != null) {
					formattedMessage.Append($" {param.Name}:{param.Value}");
				}
			}
			InfoIf(true, formattedMessage.ToString());
		}
		

	}

}