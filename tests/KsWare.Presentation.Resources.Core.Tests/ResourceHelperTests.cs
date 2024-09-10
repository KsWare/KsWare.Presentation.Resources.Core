using System;
using System.Windows;
using System.Windows.Controls;

namespace KsWare.Presentation.Resources.Core.Tests;

public class Tests {

	[SetUp]
	public void Setup() {
	}

	[Test]
	public void FindResource() {
		var d = new ResourceDictionary{Source = new Uri("pack://application:,,,/KsWare.Presentation.Resources.Core.Tests;component/Resources/Button.xaml")};
		var e = new Button {Resources = d};

		Assert.That(e.Content, Is.EqualTo("1"));
	}
}