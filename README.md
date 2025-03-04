﻿# KsWare.Presentation.Resources.Core

- [Extended ResourceDictionaries](#Extended-ResourceDictionaries)
- [ThemeLoader](#ThemeLoader)

## Extended ResourceDictionaries

| Name | Fuction |
| ---- | ------- |
|CoreResourceDictionary|A base class which extens the ResourceDictionary class|
|ResourceDictionaryEx|A feature-rich ResourceDictionary|
|ThemeResourceDictionary| Root for theme resources |
|SharedResourceDictionary| Reuseable resources |
|PlaceholderResourceDictionary | A placeholder |
|SourceResourceDictionary| The root für XAML resources |
|MergedResourceDictionary| The root for merged resources |
|LookupResourceDictionary| Lookups for an other ResourceDictionary defined as resource |
|FlatResourceDictionary| Flattens all contained resources into one ResourceDictionary|
|OverrideDefaultStylesResourceDictionary| Generates Styles|

designed to:  
- create maintainable structured themes
- avoid DynamicResource references
- fixes few issues from ResourceDictionary

### Usage

```xml
<SourceResourceDictionary
  <ResourceDictionary.MergedDictionary>
    <!-- Load shared resources -->
    <SharedResourceDictionary Source="shared.xaml

    <!-- define overridable brushes -->
    <MergedResourceDictionary Name="Brushes">
        <SolidColorBrush x:Key="Acme.Control.Border" Color="Gree"/>
        ...
    </MergedResourceDictionary>

    <!-- lookup for override -->
    <LookupResourceDictionary ResourceKey="Acme.Brushes.Overides"/>
    
    <!-- definie your styles and other resources -->
    <MergedResourceDictionary Name="Styles">
      <Style x:Key="Acme.ControlStyle" TargetType="Control">
         ...
      </Style>
    </MergedResourceDictionary

    <!-- create default styles -->
    <OverrideDefaultStylesResourceDictionary>
      <!-- the following content is autogenerated (at runtime) -->
      <Style TargetType="Control" BasedOn="{StaticResource Acme.ControlStyle}"/>
    </OverrideDefaultStylesResourceDictionary>

  </ResourceDictionary.MergedDictionary>
</SourceResourceDictionary>
```


## ThemeLoader

A loader for themes.  
You can load any resource dictionary. RegisterTheme is otional.

App.xaml.cs / App.xaml
```xml
public partial class App : Application {
    public App() {
        // optional
        ThemeLoader.RegisterTheme("Aero2Dark.NormalColor", "/KsWare.Presentation.Themes.Aero2Dark;component/Resources/Aero2Dark.NormalColor.xaml");
        ThemeLoader.RegisterTheme("Aero2Dark","Aero2Dark.NormalColor") // alias
        ThemeLoader.RegisterTheme("Aero", "/PresentationFramework.Aero;component/themes/Aero.NormalColor.xaml");
    }
```
```xml
<Application 
	...
	ThemeLoader.Source="/KsWare.Presentation.Themes.Aero2Dark;component/Resources/Aero2Dark.NormalColor.xaml"
>
```

MainWindow.cs / MainWindow.xaml.cs
```csharp
public MainWindow() {
    // optional
    ThemeLoader.RegisterTheme("Aero2Dark.NormalColor", "/KsWare.Presentation.Themes.Aero2Dark;component/Resources/Aero2Dark.NormalColor.xaml");
    ThemeLoader.RegisterTheme("Aero2Dark","Aero2Dark.NormalColor") // alias
    ThemeLoader.RegisterTheme("Aero", "/PresentationFramework.Aero;component/themes/Aero.NormalColor.xaml");

	InitializeComponent();
}
```
```xml
<Window 
  ...
  ThemeLoader.Source="Aero2Dark" >
  <!-- window content with Aero2Dark style -->
  <Grid ThemeLoader.Source="Aero">
     <!-- content with Aero style -->
  </Grid>
  <Grid ThemeLoader.Source="/PresentationFramework.Luna;component/themes/Luna.Metallic.xaml">
     <!-- content with Luna Metallic style -->
  </Grid>
</Window>
```

## License
© 2024 by KsWare. All rights reserved.  
Licensed under [KsWare Open Commercial License](LICENSE.txt).

