﻿using System;
using System.Collections.Generic;
using MyDiet.Views;
using MyDiet.Helpers;
using SQLite;
using Xamarin.Forms;
using MyDiet.Manager;

namespace MyDiet
{
    public partial class LoadingPage : ContentPage
    {
		//private SQLiteAsyncConnection _connection;
		//private AccountManager accountManager;

        public LoadingPage()
        {
            InitializeComponent();
			image.Source = ImageSource.FromResource("MyDiet.Images.bigWhite2.jpg");
			//var app = Application.Current as App;
			//if (Application.Current.Properties.ContainsKey("log"))
			//	isLoggedIn.Text = Application.Current.Properties["log"].ToString();
			//else{
			//	Application.Current.Properties["log"] = false;
			//	//isLoggedIn.Text = Application.Current.Properties["log"].ToString();

			//}
			if (Settings.LogStateSettings){
				App.email = Settings.AccountEmail;
				Navigation.PushModalAsync(new NavigationPage(new MainPage()));
			}else
            {

                Navigation.PushModalAsync(new NavigationPage(new LoginPage()));

            }
				
				
        }

		async void Handle_Clicked(object sender, System.EventArgs e)
        {
			
			continueButton.IsVisible = false;
			loadingIndicator.IsRunning = true;
			loadingIndicator.IsVisible = true;
			if (Settings.LogStateSettings)
            {
				try{
					var accountManager = new AccountManager();
					var account = await accountManager.GetAccountInfosAsync(Settings.AccountEmail);
                    App.account = account;
					
				}catch{
					await DisplayAlert("Warning", "No Network available", "continue","cancel");
					continueButton.IsVisible = true;
					loadingIndicator.IsRunning = false;
					loadingIndicator.IsVisible = false;
					Settings.LogStateSettings = false;
					await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
				}
				await Navigation.PushModalAsync(new NavigationPage(new MainPage()));


            }
            else
            {

				await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));

            }

        }

    }
}
