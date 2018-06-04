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
            
            
            var temp1 = await medicineManager.GetMedicinesAsync();
            medicineListView.ItemsSource = temp1;

			var temp2 = await medicineHistoryManager.GetMedicinesAsync();
			historyListView.ItemsSource = temp2.OrderByDescending(history => history.StartTime);

		}

		protected override void OnAppearing()
        {
            base.OnAppearing();
			if(!(Settings.ReminderDate.Month==DateTime.Now.Month && Settings.ReminderDate.Day==DateTime.Now.Day
			     && Settings.ReminderDate.Minute == DateTime.Now.Minute))
			{
				UpdateCheck();
			}

			if(App.contentChanged)
			{
				Init();
				App.contentChanged = false;
			}
    //        if (currentView == 0)
    //        {
				//ReminderClicked(); 

    //        }
    //        if (currentView == 1)
				//MedicineClicked();

            //if (currentView == 2)
                //HistoryClicked();
                     
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
			//var response = await DisplayActionSheet("Which to add?", "Cancel", null, "New Reminder", "New Medicine");
			//if (response == "New Reminder"){
			//	Reminder reminder = null;
			//	await Navigation.PushAsync(new AddReminderPage(reminder));
			//}
               
			//if (response == "New Medicine"){
			//	Medicine medicine =null;

			//	await Navigation.PushAsync(new AddMedicinePage(medicine));
			//}
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


        //*********** reminder edit click ****************
		//void ReminderEditClicked(object sender, System.EventArgs e)
   //     {
			//var reminder1 = (sender as MenuItem).CommandParameter as Reminder;
			//Navigation.PushAsync(new AddReminderPage(reminder1));

        //}

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
			//var temp =await reminderManager.GetReminderAsync();
			//reminderListView.ItemsSource = temp;

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

			//List<Reminder> reminders = new List<Reminder>();
   //         Reminder reminder1 = new Reminder
   //         {
			//	MedicineName = "enalapril(Vasotec)",
   //             Time = new TimeSpan(9, 00, 00),
			//	Quantity = "2 Tablet"
   //         };
   //         reminder1.SetTimeToDisplay();

   //         Reminder reminder2 = new Reminder
   //         {
			//	MedicineName = "Eplerenone (Inspra)",
   //             Time = new TimeSpan(12, 00, 00),
   //             Quantity = "1 Tablet"
   //         };
   //         reminder1.SetTimeToDisplay();
   //         Reminder reminder3 = new Reminder
   //         {
			//	MedicineName = "enalapril(Vasotec)",
   //             Time = new TimeSpan(19, 00, 00),
			//	Quantity = "2 Tablet"
   //         };
   //         reminder1.SetTimeToDisplay();
   //         reminder2.SetTimeToDisplay();
   //         reminder3.SetTimeToDisplay();

   //         reminders.Add(reminder1);
   //         reminders.Add(reminder2);
   //         reminders.Add(reminder3);
			//reminderListView.ItemsSource = reminders;
            
        }

		void MedicineClicked(object sender, System.EventArgs e)
        {
			if (currentView != 1)
			    MedicineClicked();
            
        }

		void MedicineClicked()
        {
			//var temp =await medicineManager.GetMedicinesAsync();
			//medicineListView.ItemsSource = temp;

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

			//List<Medicine> medicines = new List<Medicine>();
			//Medicine medicine1 = new Medicine
   //         {
			//	MedicineName = "enalapril(Vasotec)",
			//	Description="This is a medicine for lowering blood pressure",
			//	Unit = "Tablet"
   //         };

			//Medicine medicine2 = new Medicine
   //         {
			//	MedicineName = "Eplerenone (Inspra)",
			//	Description = "Treat heart disease",
			//	Unit = "Tablet"
   //         };

			//Medicine medicine3 = new Medicine

   //         {
			//	MedicineName = "Lomotil",
			//	Description = "Treat diarrhea",
			//	Unit = "Tablet"
   //         };
           

			//medicines.Add(medicine1);
			//medicines.Add(medicine2);
			//medicines.Add(medicine3);
			//medicineListView.ItemsSource = medicines;

                
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
                historyListView.ItemsSource = temp2;


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


    }
}
