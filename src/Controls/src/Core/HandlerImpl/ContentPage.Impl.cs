﻿using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls
{
	public partial class ContentPage : IPage
	{
		// TODO ezhart That there's a layout alignment here tells us this hierarchy needs work :) 
		public Primitives.LayoutAlignment HorizontalLayoutAlignment => Primitives.LayoutAlignment.Fill;

		public IView View => Content;

		// TODO ezhart super sus
		public Thickness Margin => Thickness.Zero;

		internal override void InvalidateMeasureInternal(InvalidationTrigger trigger)
		{
			IsArrangeValid = false;
			base.InvalidateMeasureInternal(trigger);
		}

		public override bool IsMeasureValid 
		{
			get 
			{ 
				return base.IsMeasureValid && View.IsMeasureValid; 
			}

			protected set => base.IsMeasureValid = value; 
		}

		public override bool IsArrangeValid 
		{ 
			get 
			{
				return base.IsArrangeValid && View.IsArrangeValid; 
			} 

			protected set => base.IsArrangeValid = value; 
		}

		protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
		{
			var width = widthConstraint;
			var height = heightConstraint;

#if WINDOWS
			// TODO ezhart Hmmmm......

			if (double.IsInfinity(width))
			{
				width = 800;
			}

			if (double.IsInfinity(height))
			{
				height = 800;
			}
#endif

			if (View is IFrameworkElement fe)
			{
				fe.Measure(width, height);
			}

			IsMeasureValid = true;
			return new Size(width, height);
		}

		protected override void ArrangeOverride(Rectangle bounds)
		{
			if (IsArrangeValid)
			{
				return;
			}

			IsArrangeValid = true;
			IsMeasureValid = true;
			Arrange(bounds);

			if (View is IFrameworkElement element)
			{
				element.Arrange(Frame);
			}

			if (View is Layout layout)
			{
				// Force layout resolution if this is a Forms layout
				// TODO ezhart Not sure this should be happening here; the renderer itself should be doing this. Investigate.
				layout.ResolveLayoutChanges();
			}

			Handler?.SetFrame(Frame);
		}
	}
}
