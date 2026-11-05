using System;
using System.Collections.Generic;
using System.Text;

using CapstoneProjectPUSH.Models;
using CapstoneProjectPUSH.Services;


namespace CapstoneProjectPUSH.ViewModels.Admins
{

    [QueryProperty(nameof(Id), "UserId")]
    internal class UsersDetailsViewModel
    {
        private readonly UserService _userService;

        public UsersDetailsViewModel(UserService userService)
        {
            _userService = userService;
        }

        [ObservableProperty]
        private int Id;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasEmail))]
        [NotifyPropertyChangedFor(nameof(HasDisplayName))]
        private User? user;




    }
}
