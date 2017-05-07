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
		string urlEntryText;
		ActivityIndicator indicator;
		HttpClient httpClient;

		public MainPage()
		{
			Title = "Asynchronous Example";

			string defaultUrl = "http://example.com";

			// Initalise a HttpClient to deal with connecting to the webpage
			httpClient = new HttpClient();

			// Text entry to get the URL to use
			urlEntry = new Entry();

			// Initialise entry and associated url string with default url
			urlEntry.Text = defaultUrl;
			urlEntryText = defaultUrl;

			// Add event handler to update url string when text entry is changed
			urlEntry.TextChanged += (sender, e) => urlEntryText = e.NewTextValue;

			// Button to initiate download asynchronously
			Button asyncDownloadButton = new Button();
			asyncDownloadButton.Text = "Async Download";
			asyncDownloadButton.HorizontalOptions = LayoutOptions.FillAndExpand;

			// Add event handler to initiate async download when button is clicked
			asyncDownloadButton.Clicked += AsyncDownloadClicked;

			// Button to initiate download asynchronously
			Button synchDownloadButton = new Button();
			synchDownloadButton.Text = "Synch Download";
			synchDownloadButton.HorizontalOptions = LayoutOptions.FillAndExpand;

			// Add event handler to initiate async download when button is clicked
			synchDownloadButton.Clicked += SynchDownloadClicked;

			// Editor to display log of background processes.
			logEditor = new Editor();
			logEditor.Text = "Initialised.\n";
			logEditor.VerticalOptions = LayoutOptions.FillAndExpand;

			// Editor to display downloaded HTML
			htmlEditor = new Editor();
			htmlEditor.Text = "HTML will appear here.\n";
			htmlEditor.VerticalOptions = LayoutOptions.FillAndExpand;

			// Activity indicator to indicate that download is in progress
			indicator = new ActivityIndicator();
			indicator.IsRunning = false;

			// Add spacing around the edges of UI, and add more to the top on iOS to compensate for status bar
			Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

			StackLayout buttonIndicatorLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Children = { synchDownloadButton, asyncDownloadButton, indicator }
			};

			// Layout the UI components in a vertical list
			StackLayout layout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Children = { urlEntry, buttonIndicatorLayout, htmlEditor, logEditor }
			};

			Content = layout;
		}

		// Run when the download button is clicked
		void SynchDownloadClicked(object sender, EventArgs e)
		{
			logEditor.Text += "\nSynchronous Download Button Clicked!\n";

			// Attempt to download the HTML
			try
			{
				logEditor.Text += "Starting Synchronous HTML download...\n";

				// Stop activity indicator running to show download is in progress
				indicator.IsRunning = true;

				// Get HTML asynchronously, but block main application thread until it completes
				string content = httpClient.GetStringAsync(urlEntryText).Result;

				// Stop activity indicator running to show download is finished
				indicator.IsRunning = false;

				logEditor.Text += "Finished Synchronous HTML download...\n";

				// Once the contentTask completes, we can calculate the length of the HTML returned
				int length = content.Length;

				// We can then print the result to the log
				logEditor.Text += "Length of returned HTML = " + length + "\n";

				htmlEditor.Text = content;
			}
			// Catch exception if website cannot be reached.
			catch (HttpRequestException)
			{
				// Stop activity indicator running to show download is unsuccessful
				indicator.IsRunning = false;

				logEditor.Text += "URL is invalid or cannot reach website.\n";
			}

			logEditor.Text += "Synchronous Download Button click handled!\n";
		}

		// Run when the download button is clicked
		async void AsyncDownloadClicked(object sender, EventArgs e)
		{
			logEditor.Text += "\nAsynchronous Download Button Clicked!\n";

			// Run GetLengthHTML asynchronously in the background, return control to main UI thread until LogHTML returns
			await LogHTML(urlEntryText);

			logEditor.Text += "Asynchronous Download Button click handled!\n";
		}

		// Asynchronously log the length of the HTML for a given URL
		public async Task LogHTML(string url)
		{
			// Attempt to download the HTML
			try
			{
				// GetStringAsync is an asynchronous method, so returns an object of type Task<TResult>
				Task<string> contentTask = httpClient.GetStringAsync(url);

				logEditor.Text += "Starting Asynchronous Download HTML download...\n";

				// Start activity indicator running to show download is in progress
				indicator.IsRunning = true;

				// 'await' keyword returns control to calling method, allowing the asynchronous method to run in a different thread
				string content = await contentTask;

				// Stop activity indicator running to show download is finished
				indicator.IsRunning = false;

				logEditor.Text += "Finished Asynchronous HTML download...\n";

				// Once the contentTask completes, we can calculate the length of the HTML returned
				int length = content.Length;

				// We can then print the result to the log
				logEditor.Text += "Length of returned HTML = " + length + "\n";

				// Print the entire HTML to the html editor box
				htmlEditor.Text = content;
			}
			// Catch exception if website cannot be reached.
			catch (HttpRequestException)
			{
				// Stop activity indicator running to show download is unsuccessful
				indicator.IsRunning = false;

				logEditor.Text += "URL is invalid or cannot reach website.\n";
			}
		}
	}
}

