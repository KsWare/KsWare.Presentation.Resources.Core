using System.Reflection;
using System.Windows.Markup;

//[assembly: XmlnsDefinition(KsWare.Presentation.Themes.Core.AssemblyInfo.XmlNamespace, "KsWare.Presentation.Themes.Core")]
//[assembly: XmlnsPrefix(KsWare.Presentation.Themes.Core.AssemblyInfo.XmlNamespace, "themes")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "KsWare.Presentation.Themes.Core")]


// namespace must equal to assembly name
// ReSharper disable once CheckNamespace
namespace KsWare.Presentation.Themes.Core {

	public static class AssemblyInfo {

		public static Assembly Assembly => Assembly.GetExecutingAssembly();

		public const string XmlNamespace = "http://ns.ksware.de/Presentation/Themes/Core";

		public const string RootNamespace = "KsWare.Presentation.Themes.Core";
	}
}

