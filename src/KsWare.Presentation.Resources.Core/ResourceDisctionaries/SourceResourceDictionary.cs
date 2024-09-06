using System;

namespace KsWare.Presentation.Resources.Core {

	/// <summary>
	/// SourceResourceDictionary is used as root element to define the XAML.
	/// </summary>
	/// <remarks>SourceResourceDictionary does not have a <c>Source</c>-property instead SourceResourceDictionary contains the source, use this as root element only.</remarks>
	/// <example>
	/// <code>
	/// &lt;SourceResourceDictionary 
	///     xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	///     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" &gt;
	///     &lt;ResourceDictionary.MergedDictionaries&gt;
	///         &lt;MergedResourceDictionary Source="/KsWare.Presentation.Themes.Aero2Dark;component\Resources\Aero2Dark\Aero2Dark.xaml"/&gt;
	///     &lt;/ResourceDictionary.MergedDictionaries&gt;
	///     &lt;Style x:Key="Skin.Button" TargetType="Button" BasedOn="{StaticResource Aero2Dark.ButtonStyle}"/&gt;
	///  &lt;/SourceResourceDictionary&gt;
	/// </code>
	/// </example>
	public class SourceResourceDictionary : ResourceDictionaryEx {

		[Obsolete("Not supported",true)]
		public override Uri Source { get; set; }

		public override void BeginInit() {
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
		}
	}
}
