using System.Net;
using System.Text.Json;
using Xunit;
using Microsoft.Playwright;

namespace UserManagement.Tests
{
    public class CreateUserTest : IAsyncLifetime
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
        public async Task TestCreateUser()
        {
            var page = await _browser.NewPageAsync();

            try
            {
                // POST: Create a user
                var newUser = new
                {
                    firstName = "TEST",
                    lastName = "TSET",
                    email = "jane.doe@example.com",
                    userName = "TEST",
                    passWord = "securepassword123",
                    accessLevel = 1
                };

                var postResponse = await page.APIRequest.PostAsync($"{_apiBaseUrl}/users", new APIRequestContextOptions
                {
                    DataObject = newUser
                });

                Assert.Equal((int)HttpStatusCode.Created, postResponse.Status); // Validate the POST success

                // Get the response from POST (with an ID)
                var createdUser = JsonSerializer.Deserialize<Dictionary<string, object>>(await postResponse.TextAsync());
                Assert.NotNull(createdUser);
                Assert.NotNull(createdUser?["id"]); // Use null-conditional operator
            }
            finally
            {
                await page.CloseAsync();
            }
        }
    }
}
