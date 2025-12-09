using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Client.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "authToken";

    public CustomAuthStateProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);

        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);

        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));

        NotifyAuthenticationStateChanged(authState);
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        return keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}