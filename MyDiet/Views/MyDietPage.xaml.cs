﻿using System;
using System.Collections.Generic;
using MyDiet.Manager;
using MyDiet.Models;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;

namespace MyDiet.Views
{
    public partial class MyDietPage : ContentPage
    {
		DietManager dietManager;

		public MyDietPage()
        {
            InitializeComponent();
            dietManager = DietManager.DefaultManager;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
           
			//await RefreshItems(true, syncItems: false);//show activity indicator and not sync
            listView.ItemsSource = await dietManager.GetTodoItemsAsync();



        }
       

        async void OnItemAdded(object sender, EventArgs e)
        {
            DietItem diet = null;
            await Navigation.PushAsync(new DietItemPage(diet)
              );
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var diet = e.SelectedItem as DietItem;

            await Navigation.PushAsync(new DietItemPage(diet));
        }

		public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }
        

		public async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
				listView.ItemsSource = await dietManager.GetTodoItemsAsync(syncItems);
            }
        }


		private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }


    }
}
