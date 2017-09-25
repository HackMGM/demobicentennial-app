using System;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Net.Http;

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
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"http://matrixrevolutions.ddns.net:8080/");
            var json = await client.GetStringAsync($"demobicentennial/?q=mobileapi/node/2.json");
            
            dynamic stuff = JsonConvert.DeserializeObject(json);
            double lat = stuff.field_coordinates.und[0].safe_value;

            double longitude = stuff.field_coordinates.und[1].safe_value;
            await Navigation.PushAsync(new MapPage(longitude, lat));
        }

	    
    }
}
