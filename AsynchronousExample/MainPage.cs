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

			// Text entry to get the URL to use
			urlEntry = new Entry();

			// Initialise entry and associated url string with default url
			urlEntry.Text = defaultUrl;
			urlEntryText = defaultUrl;

			// Add event handler to update url string when text entry is changed
			urlEntry.TextChanged += (sender, e) => urlEntryText = e.NewTextValue;

			// Button to initiate download
			Button download = new Button();
			download.Text = "Download";

			// Add event handler to initiate async download when button is clicked
			download.Clicked += DownloadClicked;

			// Editor to display log of background processes.
			logEditor = new Editor { Text = "Initialised.\n" };
			logEditor.VerticalOptions = LayoutOptions.FillAndExpand;

			// Editor to display downloaded HTML
			htmlEditor = new Editor();
			htmlEditor.VerticalOptions = LayoutOptions.FillAndExpand;
			htmlEditor.Text = "HTML will appear here.";

			// Add padding around the edges of UI, and add more to the top on iOS to compensate for status bar
			Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			// Layout the UI components in a vertical list
			StackLayout layout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children = { urlEntry, download, htmlEditor, logEditor }
			};

			Content = layout;
		}

		// Run when the download button is clicked
		async void DownloadClicked(object sender, EventArgs e)
		{
			logEditor.Text += "\nButton Pressed!\n";

			// Run GetLengthHTML asynchronously in the background
			int lengthResult = await GetLengthHTML(urlEntryText);

			// Once GetLengthHTML has completed, print the result to the log
			logEditor.Text += "Length of returned HTML = " + lengthResult + "\n";
		}

		// Asynchronously get the length of the HTML for a given URL
		public async Task<int> GetLengthHTML(string url)
		{
			// Create a HttpClient to deal with connecting to the webpage
			HttpClient httpClient = new HttpClient();

			// Attempt to download the HTML
			try
			{
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
			// Catch exception if website cannot be reached.
			catch (HttpRequestException)
			{
				logEditor.Text += "URL is invalid or cannot reach website.\n";
				return 0;
			}
		}
	}
}

