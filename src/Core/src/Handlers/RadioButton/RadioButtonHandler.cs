﻿namespace Microsoft.Maui.Handlers
{
	public partial class RadioButtonHandler
	{
		public static IPropertyMapper<IRadioButton, RadioButtonHandler> Mapper = new PropertyMapper<IRadioButton, RadioButtonHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IRadioButton.IsChecked)] = MapIsChecked,
			[nameof(ITextStyle.CharacterSpacing)] = MapCharacterSpacing,
			[nameof(ITextStyle.Font)] = MapFont,
			[nameof(ITextStyle.TextColor)] = MapTextColor,
			[nameof(IRadioButton.Content)] = MapContent,
			[nameof(IRadioButton.BorderColor)] = MapBorderColor,
			[nameof(IRadioButton.BorderWidth)] = MapBorderWidth,
			[nameof(IRadioButton.CornerRadius)] = MapCornerRadius,
		};

		public RadioButtonHandler() : base(Mapper)
		{

		}

		public RadioButtonHandler(IPropertyMapper mapper) : base(mapper ?? Mapper)
		{

		}
	}
}