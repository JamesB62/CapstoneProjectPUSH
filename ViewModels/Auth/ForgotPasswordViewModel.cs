namespace CapstoneProjectPUSH.ViewModels.Auth
{
    public partial class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly PasswordResetService _passwordResetService;

        [ObservableProperty]
        public partial string Email { get; set; }

        [ObservableProperty]
        public partial string Code { get; set; }

        [ObservableProperty]
        public partial string NewPassword { get; set; }

        [ObservableProperty]
        public partial string ConfirmPassword { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmailStep))]
        public partial bool IsCodeStep { get; set; }

        public bool IsEmailStep => !IsCodeStep;

        public ForgotPasswordViewModel(PasswordResetService passwordResetService, UserService userService) : base(userService)
        {
            _passwordResetService = passwordResetService;
            Email = string.Empty;
            Code = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        //setp one sending the email 
        [RelayCommand]
        private async Task SendCodeAsync()
        {
            if(IsBusy) return;

            if(string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email address.";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                
                bool emailExists = await _passwordResetService.IsEmailRegisteredAsync(Email);
                if (!emailExists)
                {
                    ErrorMessage = "This email address is not registered.";
                    return; 
                }

                await _passwordResetService.RequestResetAsync(Email);

                IsCodeStep = true;
            }
            catch(Exception ex)
            {
                ErrorMessage = "Somthing went worng, please try again. (errorn 667)";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        //step two vaild code and reset password 
        [RelayCommand]
        private async Task ResetPasswordAsync()
        {
            if(IsBusy) return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var result = await _passwordResetService.ResetPasswordAsync(Email, Code, NewPassword, ConfirmPassword);
                
                if(!result.Success)
                {
                    ErrorMessage = result.Message;
                    return;
                }

                Code = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

                await Shell.Current.DisplayAlertAsync("Done", result.Message, "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch(Exception ex)
            {
                ErrorMessage = "Something when worng, please try again. (error 668)";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void UseDifferentEmail()
        {
            ErrorMessage = string.Empty;
            Code = string.Empty;
            IsCodeStep = false;
        }
        
    }
}