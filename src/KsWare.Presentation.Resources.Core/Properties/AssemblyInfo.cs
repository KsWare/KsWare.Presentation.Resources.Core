using System.Reflection;
using System.Windows.Markup;

//[assembly: XmlnsDefinition(KsWare.Presentation.Resources.Core.AssemblyInfo.XmlNamespace, "KsWare.Presentation.Resources.Core")]
//[assembly: XmlnsPrefix(KsWare.Presentation.Resources.Core.AssemblyInfo.XmlNamespace, "rd")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "KsWare.Presentation.Resources.Core")]


// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.Resources.Core {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ns.ksware.de/Presentation/Resources/Core";

		public const string RootNamespace = "KsWare.Presentation.Resources.Core";
	}
}

