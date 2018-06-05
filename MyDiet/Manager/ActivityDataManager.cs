﻿
using System;
using Microsoft.WindowsAzure.MobileServices;
using MyDiet.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;


namespace MyDiet.Manager
{
    {

        
        MobileServiceClient client;
		IMobileServiceTable<ActivityData> activityData;


		public ActivityDataManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            
			this.activityData = client.GetTable<ActivityData>();
        }
              
		public async Task<List<ActivityData>> GetActivityDataAsync()
        {
            try
            {
				//var items = await activityData.LookupAsync(id);
				//MobileServiceTableQuery<TodoItem> query = todoTable
                //.Take(3);
                //List<TodoItem> items = await query.ToListAsync();

				//IEnumerable<ActivityData> items = await activityData.Take(365);
					//.Where(data => data.UserId == App.email).ToListAsync();
                   // .ToEnumerableAsync();
				var items = activityData.IncludeTotalCount().OrderByDescending(data => data.date);
				var temp=await items.Where(data => data.UserId == App.email).ToListAsync();
				var result = new List<ActivityData>(temp);
				//var resul = new List<ActivityData>(await activityData.ToListAsync());
				System.Diagnostics.Debug.WriteLine("activitydata: "+ result.Count);
				return result;
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }
       


		public async Task SaveTaskAsync(ActivityData item, bool isNew)
        {
			
                if (isNew == true)
                {
				await activityData.InsertAsync(item);
                }
                else
                {
				await activityData.UpdateAsync(item);
                }
		    
          
        }

		public async Task DeleteTaskAsync(ActivityData item)
        {
           
			await activityData.DeleteAsync(item);


        }
       
    }
}

