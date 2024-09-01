using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static KsWare.Presentation.Themes.Core.AssemblyInfo;

namespace KsWare.Presentation.Themes.Core.Tests;

public class AssemblyInfoTests {

	[Test]
	public void TestNamespace() {
		var assembly = AssemblyInfo.Assembly;
		var assemblyName = assembly.GetName(false).Name;
		var assemblyInfoNamespace = typeof(AssemblyInfo).Namespace;
		Assert.That(assemblyInfoNamespace, Is.EqualTo(assemblyName));
	}

}
