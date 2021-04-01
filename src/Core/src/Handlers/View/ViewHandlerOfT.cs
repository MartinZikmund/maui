using System;
#if __IOS__
using NativeView = UIKit.UIView;
#elif __MACOS__
using NativeView = AppKit.NSView;
#elif MONOANDROID
using NativeView = Android.Views.View;
#elif WINDOWS
using NativeView = Microsoft.UI.Xaml.FrameworkElement;
#elif NETSTANDARD || (NET6_0 && !IOS && !ANDROID)
using NativeView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
	public abstract partial class ViewHandler<TVirtualView, TNativeView> : ViewHandler,
		IViewHandler
		where TVirtualView : class, IView
#if !NETSTANDARD || IOS || ANDROID || WINDOWS
		where TNativeView : NativeView
#else
		where TNativeView : class
#endif
	{
		protected readonly PropertyMapper _defaultMapper;
		protected PropertyMapper _mapper;
		static bool HasSetDefaults;

		protected ViewHandler(PropertyMapper mapper)
		{
			_ = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_defaultMapper = mapper;
			_mapper = _defaultMapper;
		}

		protected abstract TNativeView CreateNativeView();

		public override object? NativeView => TypedNativeView;

		protected TNativeView? TypedNativeView { get; private set; }

		protected TVirtualView? VirtualView { get; private set; }

		IView? IViewHandler.VirtualView => VirtualView;

		public NativeView? View => TypedNativeView;

		public IServiceProvider? Services => MauiContext?.Services;

		public override void SetVirtualView(IView view)
		{
			_ = view ?? throw new ArgumentNullException(nameof(view));

			if (VirtualView?.Handler != null)
				VirtualView.Handler = null;

			bool setupNativeView = VirtualView == null;

			VirtualView = view as TVirtualView;
			TypedNativeView ??= CreateNativeView();

			if (setupNativeView && TypedNativeView != null)
			{
				ConnectHandler(TypedNativeView);
			}

			if (!HasSetDefaults)
			{
				if (TypedNativeView != null)
				{
					SetupDefaults(TypedNativeView);
				}

				HasSetDefaults = true;
			}

			_mapper = _defaultMapper;

			if (VirtualView is IPropertyMapperView imv)
			{
				var map = imv.GetPropertyMapperOverrides();
				var instancePropertyMapper = map as PropertyMapper<TVirtualView>;
				if (map != null && instancePropertyMapper == null)
				{
				}
				if (instancePropertyMapper != null)
				{
					instancePropertyMapper.Chained = _defaultMapper;
					_mapper = instancePropertyMapper;
				}
			}

			_mapper.UpdateProperties(this, VirtualView);
		}

		protected virtual void ConnectHandler(TNativeView nativeView)
		{
			ConnectHandler(nativeView);
		}

		protected virtual void DisconnectHandler(TNativeView nativeView)
		{
			DisconnectHandler(nativeView);
		}

		void IViewHandler.DisconnectHandler()
		{
			if (TypedNativeView != null && VirtualView != null)
				DisconnectHandler(TypedNativeView);

			if (VirtualView != null)
				VirtualView.Handler = null;

			VirtualView = null;
		}

		public override void UpdateValue(string property)
			=> _mapper?.UpdateProperty(this, VirtualView, property);

		protected virtual void SetupDefaults(TNativeView nativeView) { }
	}
}