using Presentation.Core;
using Presentation.Models;
using System.Net.Http.Json;

namespace Presentation.ViewModels;

public sealed class WeatherViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private WeatherForecastModel[] _forecasts;

    public WeatherViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public WeatherForecastModel[] Forecasts
    {
        get => _forecasts;
        private set => SetProperty(ref _forecasts, value);
    }

    public async Task GetWeatherAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var response = await _httpClient.GetFromJsonAsync<WeatherForecastModel[]>("weatherforecast");
            Forecasts = response ?? [];
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
            Forecasts = [];
        }
        finally
        {
            IsLoading = false;
        }
    }
}
