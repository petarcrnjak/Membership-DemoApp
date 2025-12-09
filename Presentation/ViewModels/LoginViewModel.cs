using Presentation.Core;
using Presentation.Models;
using System.Net.Http.Json;

namespace Presentation.ViewModels;

public sealed class LoginViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public Func<string, Task>? OnLoginSuccess { get; set; }

    public LoginViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
        Model = new LoginModel();
    }

    public LoginModel Model { get; set; }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public async Task LoginAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                Model.Email,
                Model.Password
            });

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseModel>();

                if (authResponse?.Success == true && OnLoginSuccess != null)
                {
                    await OnLoginSuccess(authResponse.Token);
                }
                else
                {
                    ErrorMessage = authResponse?.ErrorMessage ?? "Login failed";
                }
            }
            else
            {
                ErrorMessage = "Invalid email or password. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}