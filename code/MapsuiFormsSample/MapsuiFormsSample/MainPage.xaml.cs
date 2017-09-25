using System;
using Xamarin.Forms;

namespace MapsuiFormsSample
{
    public partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
            
            // TODO: Create new label

            Label testLabel = new Label
            {
                Text = "Test Label",
                HorizontalOptions = LayoutOptions.Center
            };
            Button button = new Button
            {
                Text = "Click Me!",
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            button.Clicked += OnButtonClicked;
            ContentGrid.Children.Add(testLabel);
            ContentGrid.Children.Add(button);

        }

        async void OnButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MapPage());
        }

	    
    }
}
