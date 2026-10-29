using System;
using System.Collections.Generic;
using System.Text;

namespace CapstoneProjectPUSH.ViewModels.Admins
{
    public class AdminDashboardViewModel
    {



        public AdminDashboardViewModel()
        {
        }
        public async Task LoadAsync()
        {
            // Load any necessary data for the admin dashboard here
            await Task.CompletedTask;
        }

        public async Task NavigateToTrainersAsync()
        {
            await Shell.Current.GoToAsync("//TrainersPage");
        }

        public async Task NavigateToHomeAsync()
        {
            await Shell.Current.GoToAsync("//HomePage");
        }




    }
}
