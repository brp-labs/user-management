using System.Net;
using System.Text.Json;
using Xunit;
using Microsoft.Playwright;

namespace UserManagement.Tests
{
    public class GetAllUsersTest : IAsyncLifetime
    {
        private IPlaywright _playwright = null!;
        private IBrowser _browser = null!;
        private string _apiBaseUrl = "http://localhost:5000"; // The API base URL

        public async Task InitializeAsync()
        {
           // Launch Playwright and the browser
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true // Run headless, set to false for debugging
            });
        }

        public async Task DisposeAsync()
        {
           // Close the browser and clean up Playwright
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        [Fact]
        public async Task TestGetAllUsers()
        {
            var page = await _browser.NewPageAsync();

            try
            {
                // Get all users
                var getResponse = await page.APIRequest.GetAsync($"{_apiBaseUrl}/users");

                Assert.Equal((int)HttpStatusCode.OK, getResponse.Status); // Validate GET success

                // Get the response
                var users = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(await getResponse.TextAsync());
                Assert.NotNull(users);
                Assert.True(users?.Count > 0); // Confirm that there is at least one user
            }
            finally
            {
                await page.CloseAsync();
            }
        }
    }
}
