﻿using System;
using MyDiet.Views;
using Xamarin.Forms;

namespace MyDiet
{
    public class MainPage : TabbedPage
    {
		readonly Page dietPage, homePage, medicinePage, activityPage, profilePage;
        public MainPage()
        {
            
            

			dietPage = new MyDietPage()
            {
                Title = "Diet"
            };
            
                        homePage = new HomePage()
                        {
                            Title = "Home"
                        };
   
                        medicinePage = new MedicinePage()
                        {
                            Title = "Medication"
                        };
            
                        activityPage = new ActivityPage()
                        {
                            Title = "Exercise"
                        };

                        profilePage = new ProfilePage()
                        {
                            Title = "Profile"
                        };

			if (Device.RuntimePlatform == Device.iOS)
			{
				profilePage.Icon = "tab_about.png";
				dietPage.Icon = "tab_feed.png";
				homePage.Icon = "tab_home30.png";
				medicinePage.Icon = "tab_pill30.png";
				activityPage.Icon = "tab_activity30.png";
			}


            Children.Add(homePage);
            
			Children.Add(dietPage);
            Children.Add(medicinePage);
            Children.Add(activityPage);
            Children.Add(profilePage);

            Title = Children[0].Title;
        }
	


        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            Title = CurrentPage?.Title ?? string.Empty;
        }


		public void SwitchToDiet()
        {
			CurrentPage = dietPage;
        }

        public void SwitchToMedicine()
        {
			CurrentPage = medicinePage;
        }

        
        
    }
}
