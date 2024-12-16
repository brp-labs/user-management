using System.Net;
using System.Text.Json;
using Xunit;
using Microsoft.Playwright;

namespace UserManagement.Tests
{
    public class GetUserByIdTest : IAsyncLifetime
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
        public async Task TestGetUserById()
        {
            var page = await _browser.NewPageAsync();

            try
            {
                // ATTENTION! Be sure, that a user exists with an ID equal to the userId used below
                int userId = 2;

                // Get the user by ID
                var getResponse = await page.APIRequest.GetAsync($"{_apiBaseUrl}/users/{userId}");

                Assert.Equal((int)HttpStatusCode.OK, getResponse.Status); // Validate GET success

                // Get the response (with an ID)
                var user = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(await getResponse.TextAsync());
                Assert.NotNull(user);
                Assert.Equal(userId, user?["id"].GetInt32()); // Convert JsonElement to int and confirm that the correct ID is retrieved
            }
            finally
            {
                await page.CloseAsync();
            }
        }
    }
}
