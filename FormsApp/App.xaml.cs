using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace FormsApp
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			//MainPage = new MyMasterDetailPage();
			//MainPage = new StackLayoutPage();

			//var mainPage = new CarouselPage()
			//{
			//	Children = {
			//		new CarouselSubPage(Color.Green, "olympus"),
			//		new CarouselSubPage(Color.Red, "Login"),
			//		new CarouselSubPage(Color.Yellow, "QR Code"),
			//		new CarouselSubPage(Color.Green, "Video Player"),
			//		new StackLayoutExample(),
			//		//new MyMasterDetailPagePage(),
			//	}
			//};

			var masterPage = new MyMasterDetailPage();

			MainPage = masterPage;
		}

		/// <summary>
		/// Carousel sub page.
		/// </summary>
		class CarouselSubPage : ContentPage
		{
			public CarouselSubPage(Color color, string title)
			{
				BackgroundColor = color;
				Content = new Label
				{
					//テキストを中央に表示する
					Text = title,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center
				};
			}
		}

		/// <summary>
		///ボタン遷移用のサブページ
		/// </summary>
		class SubPage : ContentPage
		{
			public SubPage(Color color, string title)
			{
				var button = new Button()
				{
					Text = "back",
				};
				BackgroundColor = color;
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						new Label
						{
							//テキストを中央に表示する
							Text = title,
							HorizontalOptions = LayoutOptions.Center,
							VerticalOptions = LayoutOptions.Center
						},
						button,
					}
				};
				button.Clicked += async (sender, e) =>
				{
					await Navigation.PopModalAsync();
				};
			}
		}

		/// <summary>
		/// My page.
		/// WebView Sample.
		/// </summary>
		class MyPage : ContentPage
		{
			public MyPage()
			{
				var webView = new WebView
				{ // <-1
					Source = "http://xamarin.com" // <-2
				};
				Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);
				Content = webView; // <-3
			}
		}

		/// <summary>
		/// Stack layout example.
		/// </summary>
		class StackLayoutExample : ContentPage
		{
			public StackLayoutExample()
			{
				Padding = new Thickness(20);
				var red = new Label
				{
					Text = "Stop",
					BackgroundColor = Color.Red,
					Font = Font.SystemFontOfSize(20)
				};
				var yellow = new Label
				{
					Text = "Slow down",
					BackgroundColor = Color.Yellow,
					Font = Font.SystemFontOfSize(20)
				};
				var green = new Label
				{
					Text = "Go",
					BackgroundColor = Color.Green,
					Font = Font.SystemFontOfSize(20)
				};

				var listView = new ListView
				{
					RowHeight = 40
				};

				// ListViewにアイテム追加
				//listView.ItemsSource = new string[]
				//	{
				//		"Buy pears",
				//		"Buy oranges",
				//		"Buy mangos",
				//		"Buy apples",
				//		"Buy bananas"
				//	};


				//ListViewsにアイテムを追加
				listView.ItemsSource = new TodoItem[]{
					new TodoItem{Name = "Buy pears", Done = false},
					new TodoItem {Name = "Buy oranges", Done=true},
					new TodoItem {Name = "Buy mangos"},
					new TodoItem {Name = "Buy apples", Done=true},
					new TodoItem {Name = "Buy bananas", Done=true}
				};
				// ListViewにどのプロパティを表示するか、バインディングする
				listView.ItemTemplate = new DataTemplate(typeof(TextCell));
				listView.ItemTemplate.SetBinding(TextCell.TextProperty, "Name");

				// 簡易アラート
				//listView.ItemSelected += async (sender, e) => {
				//	await DisplayAlert("Tapped!", e.SelectedItem + " was tapped.", "OK");
				//};

				// ページ遷移
				listView.ItemSelected += async (sender, e) =>
				{
					var todoItem = (TodoItem)e.SelectedItem;
					var todoPage = new SubPage(Color.Red, todoItem.Name); // so the new page shows correct data
					await Navigation.PushModalAsync(todoPage);
				};

				Content = new StackLayout
				{
					//Spacing = 10,
					//Children = { red, yellow, green }
					Children = { listView },
				};
			}
		}

		/// <summary>
		/// MasterDetailPage.
		/// </summary>
		class MyMasterDetailPage : MasterDetailPage
		{
			public MyMasterDetailPage()
			{
                // スキャンボタン
                var button1 = new Button
                {
                    Text = "Scan",
                };
                button1.Clicked += ScanButtonClicked;

                var ar = new[] { "VideoPlayer", "StoredItems", "Setting" };

				ListView listView = new ListView
				{
					ItemsSource = ar,
					//BackgroundColor = Color.Transparent
				};

                // ツールバーアイテム
                this.ToolbarItems.Add(new ToolbarItem()
                {
                    Name = "SCAN",
                    Command = new Command(async () => await ScanCommand())
                });

				// マスターページ
				this.Master = new ContentPage
				{
					BackgroundColor = Color.FromRgba(0.86, 0.91, 0.94, 0.5),

					//iPhoneにおいて、ステータスバーとの重なりを防ぐためパディングを調整する
					Padding = new Thickness(0, Device.OnPlatform(20, 20, 0), 0, 0),
					Title = "Master", // 必須
					Icon = "menu.png",
					Content = new StackLayout()
                    {
                        Children = {button1, listView }
                    },
				};

				// リストが選択された際のイベント処理
				listView.ItemSelected += (s, a) =>
				{
					//プロパティDetailに新しいページをセットする
					Detail = new NavigationPage(new DetailPage(a.SelectedItem.ToString()))
					{
						//  タイトルバーの背景色や文字色は、NavigationPageのプロパティをセットする
						BarBackgroundColor = Color.FromRgba(0.2, 0.6, 0.86, 1),
						BarTextColor = Color.White
					};
					IsPresented = false;//  Detailページを表示する
				};

				listView.SelectedItem = ar[0];// 必須　最初のページをセットする

			}

            public async Task ScanCommand()
            {
                var scanPage = new ZXingScannerPage()
                {
                    DefaultOverlayTopText = "バーコードを読み取ります",
                    DefaultOverlayBottomText = "",
                };
                await Navigation.PushModalAsync(scanPage);

                scanPage.OnScanResult += (result) =>
                {
                    scanPage.IsScanning = false;

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopModalAsync();
                        await DisplayAlert("", result.Text, "OK");
                    });
                };
            }

            async void ScanButtonClicked(object sender, EventArgs s)
            {
                var scanPage = new ZXingScannerPage()
                {
                    DefaultOverlayTopText = "バーコードを読み取ります",
                    DefaultOverlayBottomText = "",
                };
                await Navigation.PushModalAsync(scanPage);

                scanPage.OnScanResult += (result) =>
                {
                    scanPage.IsScanning = false;

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopModalAsync();
                        await DisplayAlert("", result.Text, "OK");
                    });
                };
            }
            
        }

		//詳細ページ
		class DetailPage : ContentPage
		{
			public DetailPage(string title)
			{
				Title = title;
				Content = new Label
				{
					//テキストを中央に表示する
					Text = title,
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
				};

			}
		}

		/// <summary>
		/// Todo item.
		/// </summary>
        /// 
		class TodoItem
		{
			public string Name
			{
				get;
				set;
			}
			public bool Done
			{
				get;
				set;
			}
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
