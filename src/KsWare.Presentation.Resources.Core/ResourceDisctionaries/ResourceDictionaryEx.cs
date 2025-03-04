﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using KsWare.Presentation.Core.Utils;
using KsWare.Presentation.Resources.Core.Internal;

namespace KsWare.Presentation.Resources.Core {

	/// <summary>
	/// Extended ResourceDictionary
	/// </summary>
	[UsableDuringInitialization(true)]
    public class ResourceDictionaryEx : CoreResourceDictionary {
	    private static int s_nextId;
		internal static readonly List<ResourceDictionaryEx> InitStack = new List<ResourceDictionaryEx>();
		internal static readonly List<ResourceDictionaryEx> EndInitList = new List<ResourceDictionaryEx>();

		internal static ResourceDictionaryEx LastRootDictionary { get; private set; }
		internal static List<WeakReference<ResourceDictionaryEx>> RootDictionaries{get;} = new List<WeakReference<ResourceDictionaryEx>>();
	    private protected static RdTrace Trace = new RdTrace();

	    private bool _onGettingValueInvoked;
	    private ThemeResourceDictionary _themeDictionary;

	    public ResourceDictionaryEx() {
		    InstanceId = System.Threading.Interlocked.Increment(ref s_nextId);
		    IsRootDictionary = InitStack.Count == 0;
		    if (IsRootDictionary) {
			    LastRootDictionary = this;
			    RootDictionaries.Add(new WeakReference<ResourceDictionaryEx>(this));
		    }
		    else RootDictionary = LastRootDictionary;
		    if (this is ThemeResourceDictionary) {
			    if (ThemeResourceDictionary.Current != null && ThemeResourceDictionary.Current.IsInitializing) throw new InvalidOperationException("Only one ThemeResourceDictionary can be initialized at a time.");
			    ThemeResourceDictionary.Current = (ThemeResourceDictionary)this;
		    }
//		    Trace.Info($"{GetType().Name}({InstanceId}).ctor {Application.Current.Resources.Count} {Application.Current.Resources.MergedDictionaries.Count}");
		    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Send, new Action(PostInit));
	    }

	    public int InstanceId { get; }
	    public bool IsRootDictionary { get; }
	    public ResourceDictionaryEx RootDictionary { get; set; }

	    public TraceInfoData? TraceInfo {
		    get => null;
		    set {
			    Trace.Info($"{GetType().Name}({InstanceId}).TraceInfo {value.ToString()}");
		    }
	    }

	    protected bool IsInitialized { get; private set; }
	    protected bool IsInitializing { get; private set; }
	    protected ResourceDictionaryEx? InitialParent { get; private set; }
	    protected ResourceDictionaryEx? LogicalParent { get; private set; }

	    public string Name { get; set; }

	    public override Uri? Source {
		    get => base.Source;
		    set {
				if(!IsInitializing) throw new InvalidOperationException($"Use without BeginInit is not supported. Source='{value}'");
			    Trace.Info($"{GetType().Name}({InstanceId}).SetSource {value}");
			    Location = Path.GetFileName(value.OriginalString);
			    base.Source = value;
			    Trace.Info($"{GetType().Name}({InstanceId}).SetSource (End)");
		    }
	    }

	    public string? Location { get; set; }
	    public bool EnableTrace { get => Trace.IsEnabled; set => Trace.SetIsEnabled(value, this); }
	    public bool TraceValue { get; set; }

	    protected override void OnGettingValue(object key, ref object value, out bool canCache) {
		    if (!_onGettingValueInvoked) {
			    _onGettingValueInvoked = true;
			    Trace.Info($"{GetType().Name}({InstanceId}).OnGettingValue (first time)");
		    }
//		    if (IsRootDictionary && value.GetType().FullName == "System.Windows.Baml2006.KeyRecord") {
//			    value = this[key]; // endless loop: calls OnGettingValue again with a KeyRecord
//		    }
		    if (IsRootDictionary && value.GetType().FullName == "System.Windows.Baml2006.KeyRecord") {
			    value = TryFindResource(key, this, true, true)!;
		    }
		    
			Trace.InfoIf(TraceValue, $"Read:{ResourceHelper.KeyToString(key)}");
		    base.OnGettingValue(key, ref value, out canCache);

		    switch (value) {
			    case Style style: UpdateResourceInfo(style); break; // TODO PERFORMANCE
		    }
	    }

	    public override void BeginInit() {
		    IsInitializing = true;
		    Trace.Info($"{GetType().Name}({InstanceId}).BeginInit");
		    InitialParent = InitStack.LastOrDefault();
		    LogicalParent = InitialParent != null ? (InitialParent.Source == null ? InitialParent.InitialParent : InitialParent) : null;
			InitStack.Add(this);
		    base.BeginInit();
		    if (!(this is ThemeResourceDictionary theme) && ThemeResourceDictionary.Current!=null) {
			    // WORKAROUND for StaticResourceExtension, to make resources available immediately.
//			    MergedDictionaries.Add(ThemeResourceDictionary.Current);
//				_themeDirectory = ThemeResourceDictionary.Current;
		    }
	    }

	    public override void EndInit() {
		    base.EndInit();
		    IsInitializing = false;
		    InitStack.Remove(this);
		    Trace.Info($"{GetType().Name}({InstanceId}).EndInit", ("Location", Location), ("Source", Source), ("BaseUri",((IUriContext)this).BaseUri));
		    EndInitList.Add(this);

//			possible fix for 'default style returns null'
//			if(IsRootDictionary)
//				foreach (var key in Keys.OfType<Type>().Where(k => typeof(FrameworkElement).IsAssignableFrom(k)))
//					_ = this[key]; // force reading of default styles

		    IsInitialized = true;
	    }

	    private void UpdateResourceInfo(ResourceDictionary rd) {
		    foreach (var value in Values) {
			    switch (value) {
				    case Style style: UpdateResourceInfo(style);break;
			    }
		    }
		    foreach (var md in MergedDictionaries) {
			    if(md.GetType()!=typeof(ResourceDictionary)) continue;
			    UpdateResourceInfo(md);
		    }
	    }

	    private void UpdateResourceInfo(Style style) {
			if(style.IsSealed) return;
		    var info = style.Setters.FirstOrDefault(s => s is InfoSetter ss && ss.Property==ResourceInfo.StyleLocationProperty);
			if(info!=null) return;
			info = new InfoSetter() {
				Property = ResourceInfo.StyleLocationProperty,
				Value = ((IUriContext) this).BaseUri?.ToString() ?? Source?.ToString() ?? Location
			};
			style.Setters.Insert(0,info);
	    }

	    protected virtual void PostInit() {
			Trace.Info($"{GetType().Name}({InstanceId}).PostInitialize", ("Location", Location), ("Source", Source));
		    EndInitList.Remove(this);
		    if (_themeDictionary != null) MergedDictionaries.Remove(_themeDictionary);
		}
    }

}
