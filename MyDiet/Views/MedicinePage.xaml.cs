﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyDiet.Manager;
using MyDiet.Models;
using Xamarin.Forms;
using System.Linq;
using MyDiet.Helpers;
using Plugin.LocalNotifications;
using System.Diagnostics;

namespace MyDiet.Views
{
    public partial class MedicinePage : ContentPage
    {
		

		int currentView = 0;
		MedicineManager medicineManager;
		ReminderManager reminderManager;
		MedicineHistoryManager medicineHistoryManager;
		public MedicinePage()
		{
			InitializeComponent();
			medicineManager =MedicineManager.DefaultManager;
			reminderManager = ReminderManager.DefaultManager;
			medicineHistoryManager = MedicineHistoryManager.DefaultManager;
			Init();      
			ReminderClicked();
			currentView = 0;

		}

		async public void Init()
		{
			var temp = await reminderManager.GetReminderAsync();
            reminderListView.ItemsSource = temp.OrderBy(reminder => reminder.Time);

			var temp2 = await medicineHistoryManager.GetMedicinesAsync();
			historyListView.ItemsSource = temp2.OrderByDescending(history => history.StartTime);
            
            var temp1 = await medicineManager.GetMedicinesAsync();

			foreach (var t in temp1)
			{
				t.Max = int.Parse(t.Duration);
				var startday = new DateTime(t.StartTime.Year, t.StartTime.Month, t.StartTime.Day);
				var diff = DateTime.Now - startday;

				var difff = diff.Days + 1;
				t.Status = difff + " out of " + t.Duration + " days";
				t.CurrentDay = difff;
			}
                 medicineListView.ItemsSource = temp1;




		}

		protected override void OnAppearing()
        {
            base.OnAppearing();
			if(!(Settings.ReminderDate.Month==DateTime.Now.Month && Settings.ReminderDate.Day==DateTime.Now.Day))
			    // && Settings.ReminderDate.Minute == DateTime.Now.Minute))
			{
				UpdateCheck();
			}

			if(App.contentChanged)
			{
				Init();
				App.contentChanged = false;
			}
                      
        }
        
		async public void UpdateCheck()
		{
			var temp = await reminderManager.GetReminderAsync();
			foreach(Reminder reminder1 in temp)
			{
				reminder1.Checked = false;
				reminder1.SetUnChecked();
				await reminderManager.SaveTaskAsync(reminder1, false);

			}
			Settings.ReminderDate = DateTime.Now;
			temp = await reminderManager.GetReminderAsync();
			reminderListView.ItemsSource = temp.OrderBy(reminder => reminder.Time);


		}
		void OnAdded(object sender, System.EventArgs e)
        {
			
			Medicine medicine = null;

            Navigation.PushAsync(new AddMedicinePage(medicine));
				
            
        }

        //**************** reminder Checked ******************8
		async void ReminderItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            var reminderN = e.SelectedItem as Reminder;
			if (!reminderN.Checked)
			{
				var confirm = await DisplayAlert("Notice!", "Checked status can not be modified! Are you sure to check this reminder?", "Yes", "Cancel");
				if (confirm)
				{
					reminderN.Checked = true;
					reminderN.SetUnChecked();
					await reminderManager.SaveTaskAsync(reminderN, false);
					var temp = await reminderManager.GetReminderAsync();
					reminderListView.ItemsSource = temp.OrderBy(reminder => reminder.Time.Hours);
                    //cancel today notification 
					CrossLocalNotifications.Current.Cancel(reminderN.GetHashCode());

                    //set new notificatoin for next day
					DateTime notiTime = new DateTime(
						DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, reminderN.Time.Hours, reminderN.Time.Minutes, 0
                    );
					notiTime = notiTime.AddMinutes(1);
					//notiTime = notiTime.AddDays(1);
					CrossLocalNotifications.Current.Show("Medicine Notification",
					                                     "It's time to take " + reminderN.MedicineName, reminderN.GetHashCode(),
					                                     notiTime);

				}



			}
			else
				await DisplayAlert("Notice:", "You have checked this today!", "OK");

        }

		void Handle_MedicineTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
			var medicine2 = e.Item as Medicine;

		    Navigation.PushAsync(new AddMedicinePage(medicine2));
        }
        
		void MedicineItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
			medicineListView.SelectedItem = null;
            
        }

		void Handle_HistoryTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
			var medicineHistory = e.Item as MedicineHistory;

			Navigation.PushAsync(new MedicineHistoryPage(medicineHistory));
        }

        void HistoryItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
			historyListView.SelectedItem = null;

        }


        

        //************** reminder delete ******************
		async void ReminderDeleteClicked(object sender, System.EventArgs e)
        {
			var reminder2 = (sender as MenuItem).CommandParameter as Reminder;
			var confirm = await DisplayAlert("Notice!", "Are you sure to delete this reminder?", "Yes", "Cancel");
            if (confirm)
            {
                
				await reminderManager.DeleteTaskAsync(reminder2);
				var temp = await reminderManager.GetReminderAsync();
				reminderListView.ItemsSource = temp.OrderBy(reminders => reminders.Time);
				CrossLocalNotifications.Current.Cancel(reminder2.GetHashCode());

            }

        }


		//************** history delete ******************
        async void HistoryDeleteClicked(object sender, System.EventArgs e)
        {
			var medicineHistory = (sender as MenuItem).CommandParameter as MedicineHistory;
            var confirm = await DisplayAlert("Notice!", "Are you sure to delete this medicine record?", "Yes", "Cancel");
            if (confirm)
            {

				await medicineHistoryManager.DeleteTaskAsync(medicineHistory);
				var temp2 = await medicineHistoryManager.GetMedicinesAsync();
                historyListView.ItemsSource = temp2;
            }

        }


		void ReminderClicked(object sender, System.EventArgs e)
        {
			if(currentView!=0)
    			ReminderClicked();

        }

		void ReminderClicked()
        {
			

			medicineListView.IsVisible = false;
			historyListView.IsVisible = false;
			reminderListView.IsVisible = true;

            reminder.BackgroundColor = Color.FromHex("#2196F3");
            reminder.TextColor = Color.White;
            medicine.BackgroundColor = Color.WhiteSmoke;
            medicine.TextColor = Color.FromHex("#2196F3");
            history.BackgroundColor = Color.WhiteSmoke;
            history.TextColor = Color.FromHex("#2196F3");
            currentView = 0;


            
        }

		void MedicineClicked(object sender, System.EventArgs e)
        {
			if (currentView != 1)
			    MedicineClicked();
            
        }

		void MedicineClicked()
        {
			

			reminderListView.IsVisible = false;
			historyListView.IsVisible = false;
			medicineListView.IsVisible = true;
			reminder.BackgroundColor = Color.WhiteSmoke;
			reminder.TextColor = Color.FromHex("#2196F3");
			medicine.BackgroundColor = Color.FromHex("#2196F3");
			medicine.TextColor = Color.White;
            history.BackgroundColor = Color.WhiteSmoke;
            history.TextColor = Color.FromHex("#2196F3");
            currentView = 1;
   

                
        }
        


        void HistoryClicked(object sender, System.EventArgs e)
        {
			if (currentView != 2)
              HistoryClicked();
        }
         void HistoryClicked()
        {
			
			reminderListView.IsVisible = false;
			medicineListView.IsVisible = false;
			historyListView.IsVisible = true;

			reminder.BackgroundColor = Color.WhiteSmoke;
			reminder.TextColor = Color.FromHex("#2196F3");
			medicine.BackgroundColor = Color.WhiteSmoke;
			medicine.TextColor = Color.FromHex("#2196F3");
            history.BackgroundColor = Color.FromHex("#2196F3");
            history.TextColor = Color.White;
            currentView = 2;
        }

              
        


        //************************Offline************** medicine
		public async void OnMedicineRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshMedicines(false, true);
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
				if (Device.RuntimePlatform == Device.iOS)
                    await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }


        public async void OnSyncItems()
        {
			await RefreshMedicines(true, true);
        }

		private async Task RefreshMedicines(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                
				var temp = await medicineManager.GetMedicinesAsync(syncItems);

                foreach (var t in temp)
                {
                    t.Max = int.Parse(t.Duration);
                    var startday = new DateTime(t.StartTime.Year, t.StartTime.Month, t.StartTime.Day);
                    var diff = DateTime.Now - startday;

                    var difff = diff.Days + 1;
                    t.Status = difff + " out of " + t.Duration + " days";
                    t.CurrentDay = difff;
					            
				}
                medicineListView.ItemsSource = temp;


                
                
            }
        }

        //********************offline **************for reminder
		public async void OnReminderRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshReminder(false, true);
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

        

		private async Task RefreshReminder(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {

				var temp = await reminderManager.GetReminderAsync(syncItems);
				reminderListView.ItemsSource = temp.OrderBy(reminder => reminder.Time);


            }
        }


		//********************offline **************for history
        public async void OnHistoryRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshHistory(false, true);
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



        private async Task RefreshHistory(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {

				var temp2 = await medicineHistoryManager.GetMedicinesAsync();
				historyListView.ItemsSource = temp2.OrderByDescending(history => history.StartTime);


            }
        }



        //****************show activity indicator
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



		public async void end(Medicine currentMedicine){
			
            var temp = await reminderManager.GetReminderAsync();
            foreach (var t in temp)
            {
                if (t.MedicineId == currentMedicine.Id)
                {
                    await reminderManager.DeleteTaskAsync(t);
                    DeleteNotification(t);
                }

            }
            //delete thie medicine 
            currentMedicine.IsTaking = false;
			await medicineManager.DeleteTaskAsync(currentMedicine);
            App.contentChanged = true;

            //set a new medicine history 
            MedicineHistory medicineHistory = new MedicineHistory
            {
                Id = Guid.NewGuid().ToString(),
                UserId = App.email,
                MedicineName = currentMedicine.MedicineName,
                Directions = currentMedicine.Directions,
                TimeToDisplay = currentMedicine.StartTime.ToString("dd/MMM/yyyy ddd") + " - " + DateTime.Now.ToString("dd/MMM/yyyy ddd"),
                Description = currentMedicine.Description,
                Duration = currentMedicine.Duration,
                TimesPerDay = currentMedicine.TimesPerDay,
                Unit = currentMedicine.Unit,
                StartTime = currentMedicine.StartTime,
                DirectionsToDisplay = currentMedicine.TimesPerDay + " times a day, " + currentMedicine.Unit + " each time",
				IsDone =true,
				IsUnDone=false
            };
            if (currentMedicine.TimesPerDay == 1)
            {
                medicineHistory.DirectionsToDisplay = "Once a day, " + currentMedicine.Unit + " each time";
            }
            //medicineHistoryManager = MedicineHistoryManager.DefaultManager;
            await medicineHistoryManager.SaveTaskAsync(medicineHistory, true);
            await DisplayAlert("Notice:", "You have ended this medicine!", "OK");
		}

		public void DeleteNotification(Reminder reminder)
        {
            CrossLocalNotifications.Current.Cancel(reminder.GetHashCode());
        }


    }
}
