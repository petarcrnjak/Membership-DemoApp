using Presentation.Core;
using Presentation.Models;
using System.Net.Http.Json;

namespace Presentation.ViewModels;

public sealed class RegisterViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    private string _errorMessage = string.Empty;
    private List<string>? _errors;
    private bool _isLoading;
    private bool _successMessage;

    public Func<string, Task>? OnRegisterSuccess { get; set; }

    public RegisterViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
        Model = new RegisterModel();
    }

    public RegisterModel Model { get; set; }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public List<string>? Errors
    {
        get => _errors;
        set => SetProperty(ref _errors, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    public async Task RegisterAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = false;
        Errors = null;

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", Model);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseModel>();

                if (authResponse?.Success == true && OnRegisterSuccess != null)
                {
                    SuccessMessage = true;
                    await Task.Delay(1500);
                    await OnRegisterSuccess(authResponse.Token);
                }
                else
                {
                    ErrorMessage = authResponse?.ErrorMessage ?? "Registration failed";
                    Errors = authResponse?.Errors?.ToList();
                }
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponseModel>();
                ErrorMessage = errorResponse?.Message ?? "Registration failed. Please check your input.";
                Errors = errorResponse?.Errors;
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

    private class ErrorResponseModel
    {
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}