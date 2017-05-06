using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AsynchronousExample
{
	public class MainPage : ContentPage
	{
		Editor logEditor;
		Editor htmlEditor;
		Entry urlEntry;
		// ActivityIndicator indicator;
		string urlEntryText;

		public MainPage()
		{
			Title = "Asynchronous Example";

			string defaultUrl = "http://example.com";

			urlEntry = new Entry();
			urlEntry.Text = defaultUrl;
			urlEntryText = defaultUrl;
			urlEntry.TextChanged += (sender, e) => urlEntryText = e.NewTextValue;

			Button download = new Button();
			download.Text = "Download";
			download.Clicked += DownloadClicked;

			logEditor = new Editor { Text = "Initialised.\n" };
			logEditor.VerticalOptions = LayoutOptions.FillAndExpand;

			htmlEditor = new Editor();
			htmlEditor.VerticalOptions = LayoutOptions.FillAndExpand;
			htmlEditor.Text = "HTML will appear here.";

			Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			StackLayout layout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children = { urlEntry, download, htmlEditor, logEditor }
			};

			Content = layout;
		}

		async void DownloadClicked(object sender, EventArgs e)
		{
			logEditor.Text += "\nButton Pressed!\n";

			int lengthResult = await GetLengthHTML(urlEntryText);

			logEditor.Text += "Length of returned HTML = " + lengthResult + "\n";
		}

		// Asynchronously get the length of the HTML for a given URL
		public async Task<int> GetLengthHTML(string url)
		{
			// Create a HttpClient to deal with connecting to the webpage
			HttpClient httpClient = new HttpClient();

			// GetStringAsync is an asynchronous method, so returns an object of type Task<TResult>
			Task<string> contentTask = httpClient.GetStringAsync(url);

			logEditor.Text += "Starting HTML download...\n";

			// 'await' keyword returns control to calling method, allowing the asynchronous method to run in a different thread
			string content = await contentTask;

			logEditor.Text += "Finished HTML download...\n";

			// Once the contentTask completes, we can calculate the length of the HTML returned
			int length = content.Length;

			logEditor.Text += "Got the length.\n";

			// Print the entire HTML to the log
			htmlEditor.Text = content;

			return length;
		}
	}
}

