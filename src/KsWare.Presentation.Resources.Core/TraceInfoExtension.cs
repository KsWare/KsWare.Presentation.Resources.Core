using System;
using System.IO;
using System.Text;
using System.Windows.Markup;

namespace KsWare.Presentation.Resources.Core {

	// {TraceInfo}
	// {TraceInfo Name
	// {TraceInfo Name, 'Message'}
	// <SourceResourceDictionary TraceInfo={TraceInfo 'Style Definitions'}>

	[MarkupExtensionReturnType(typeof(TraceInfoData))]
	public class TraceInfoExtension : StaticExtension {
		
		public TraceInfoExtension() {
			Message = null;
			Name = null;
		}

		public TraceInfoExtension(string name) {
			Message = null;
			Name = name;
		}

		public TraceInfoExtension(string name, string message) {
			Message = message;
			Name = name;
		}

		[ConstructorArgument("name")]
		public string? Name { get; set; }

		[ConstructorArgument("message")]
		public string? Message { get; set; }

		public override object ProvideValue(IServiceProvider? serviceProvider) {

			var service = serviceProvider?.GetService(typeof(IUriContext)) as IUriContext;

			var data = new TraceInfoData {
				Name = Name,
				Message = Message
			};

			if (service?.BaseUri != null) {
				data.Location = Path.GetFileName(service.BaseUri.LocalPath);
//				data.Name = string.IsNullOrEmpty(Name) ? _ : Name;
			}

			return data;
		}



	}

	public struct TraceInfoData {
		public string? Name { get; set; }
		public string? Message { get; set; }
		public string? Location { get; set; }

		public override string ToString() {
			var sb = new StringBuilder();
			if (Name != null) sb.Append($"Name:{Name} ");
			if (Message != null) sb.Append($"Message:{Message} ");
			if (Location != null) sb.Append($"Location:{Location} ");
			return sb.ToString();
		}
	}	
}
