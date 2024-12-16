using System.Net;
using System.Text.Json;
using Xunit;
using Microsoft.Playwright;

namespace UserManagement.Tests
{
    public class UpdateUserTest : IAsyncLifetime
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
        public async Task TestUpdateUser()
        {
            var page = await _browser.NewPageAsync();

            try
            {
                 // ATTENTION! Be sure, that a user exists with an ID equal to the userId used below
                int userId = 7;

                // PUT: Update the user
                var updatedUser = new
                {
                    firstName = "Janet",
                    lastName = "Smith",
                    email = "janet.smith@example.com",
                    userName = "janetsmith",
                    passWord = "IDE_IDE_NY_updatedpassword456",
                    accessLevel = 2
                };

                var putResponse = await page.APIRequest.PutAsync($"{_apiBaseUrl}/users/{userId}", new APIRequestContextOptions
                {
                    DataObject = updatedUser
                });

                // Confirm that PUT returns "204 No Content"
                Assert.Equal((int)HttpStatusCode.NoContent, putResponse.Status);

                // GET: Retrieve the updated user
                var getUpdatedResponse = await page.APIRequest.GetAsync($"{_apiBaseUrl}/users/{userId}");
                Assert.Equal((int)HttpStatusCode.OK, getUpdatedResponse.Status);

                // Deserialize the JSON response
                var updatedUserData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(await getUpdatedResponse.TextAsync());
                Assert.NotNull(updatedUserData);

                // Check that all keys exist before reading the values
                Assert.True(updatedUserData?.ContainsKey("firstName"), "Key 'firstName' not found in response.");
                Assert.True(updatedUserData?.ContainsKey("lastName"), "Key 'lastName' not found in response.");
                Assert.True(updatedUserData?.ContainsKey("email"), "Key 'email' not found in response.");
                Assert.True(updatedUserData?.ContainsKey("userName"), "Key 'userName' not found in response.");
                Assert.True(updatedUserData?.ContainsKey("accessLevel"), "Key 'accessLevel' not found in response.");

                // Validate the updated fields
                Assert.Equal("Janet", updatedUserData?["firstName"].GetString());
                Assert.Equal("Smith", updatedUserData?["lastName"].GetString());
                Assert.Equal("janet.smith@example.com", updatedUserData?["email"].GetString());
                Assert.Equal("janetsmith", updatedUserData?["userName"].GetString());
                Assert.Equal(2, updatedUserData?["accessLevel"].GetInt32());
            }
            finally
            {
                await page.CloseAsync();
            }
        }
    }
}
