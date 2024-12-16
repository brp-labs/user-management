using System.Net;
using System.Text.Json;
using Xunit;
using Microsoft.Playwright;

namespace UserManagement.Tests
{
    public class DeleteUserTest : IAsyncLifetime
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
        public async Task TestDeleteUser()
        {
            var page = await _browser.NewPageAsync();

            try
            {
                // ATTENTION! Be sure, that a user exists with an ID equal to the userId used below
                int userId = 9;

                // Delete the user
                var deleteResponse = await page.APIRequest.DeleteAsync($"{_apiBaseUrl}/users/{userId}");

                Assert.Equal((int)HttpStatusCode.OK, deleteResponse.Status); // Validate DELETE success

                // Confirm that the user is deleted by GetById
                var getDeletedResponse = await page.APIRequest.GetAsync($"{_apiBaseUrl}/users/{userId}");
                Assert.Equal((int)HttpStatusCode.NotFound, getDeletedResponse.Status); // Confirm than the user doesn't exist
            }
            finally
            {
                await page.CloseAsync();
            }
        }
    }
}
