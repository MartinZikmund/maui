using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Handlers;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	public partial class DatePickerHandlerTests
	{
		[Fact(DisplayName = "Minimum Date Initializes Correctly")]
		public async Task MinimumDateInitializesCorrectly()
		{
			DateTime xplatMinimumDate = new DateTime(2000, 01, 01);

			var datePicker = new DatePickerStub()
			{
				MinimumDate = xplatMinimumDate,
				Date = DateTime.Today
			};

			double expectedValue = (long)xplatMinimumDate.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;

			var values = await GetValueAsync(datePicker, (handler) =>
			{
				return new
				{
					ViewValue = datePicker.MinimumDate,
					NativeViewValue = GetNativeMinimumDate(handler)
				};
			});

			Assert.Equal(xplatMinimumDate, values.ViewValue);
			Assert.Equal(expectedValue, values.NativeViewValue);
		}

		[Fact(DisplayName = "Maximum Date Initializes Correctly")]
		public async Task MaximumDateInitializesCorrectly()
		{
			DateTime xplatMaximumDate = new DateTime(2030, 01, 01);

			var datePicker = new DatePickerStub()
			{
				MinimumDate = new DateTime(2000, 01, 01),
				MaximumDate = new DateTime(2030, 01, 01),
				Date = DateTime.Today
			};

			double expectedValue = (long)xplatMaximumDate.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;

			var values = await GetValueAsync(datePicker, (handler) =>
			{
				return new
				{
					ViewValue = datePicker.MaximumDate,
					NativeViewValue = GetNativeMaximumDate(handler)
				};
			});

			Assert.Equal(xplatMaximumDate, values.ViewValue);
			Assert.Equal(expectedValue, values.NativeViewValue);
		}

		[Theory(DisplayName = "Font Family Initializes Correctly")]
		[InlineData(null)]
		[InlineData("monospace")]
		[InlineData("Dokdo")]
		public async Task FontFamilyInitializesCorrectly(string family)
		{
			var datePicker = new DatePickerStub()
			{
				Date = DateTime.Today,
				Font = Font.OfSize(family, 10)
			};

			var handler = await CreateHandlerAsync(datePicker);
			var nativeDatePicker = GetNativeDatePicker(handler);

			var fontManager = handler.Services.GetRequiredService<IFontManager>();

			var nativeFont = fontManager.GetTypeface(Font.OfSize(family, 0.0));

			Assert.Equal(nativeFont, nativeDatePicker.Typeface);

			if (string.IsNullOrEmpty(family))
				Assert.Equal(fontManager.DefaultTypeface, nativeDatePicker.Typeface);
			else
				Assert.NotEqual(fontManager.DefaultTypeface, nativeDatePicker.Typeface);
		}

		MauiDatePicker GetNativeDatePicker(DatePickerHandler datePickerHandler) =>
			(MauiDatePicker)datePickerHandler.View;

		DateTime GetNativeDate(DatePickerHandler datePickerHandler)
		{
			var dateString = GetNativeDatePicker(datePickerHandler).Text;
			DateTime.TryParse(dateString, out DateTime result);

			return result;
		}

		long GetNativeMinimumDate(DatePickerHandler datePickerHandler)
		{
			var dialog = datePickerHandler.DatePickerDialog;
			var minDate = dialog.DatePicker.MinDate;

			return minDate;
		}

		long GetNativeMaximumDate(DatePickerHandler datePickerHandler)
		{
			var dialog = datePickerHandler.DatePickerDialog;
			var maxDate = dialog.DatePicker.MaxDate;

			return maxDate;
		}

		double GetNativeUnscaledFontSize(DatePickerHandler DatePickerHandler)
		{
			var mauiDatePicker = GetNativeDatePicker(DatePickerHandler);
			return mauiDatePicker.TextSize / mauiDatePicker.Resources.DisplayMetrics.Density;
		}
	}
}