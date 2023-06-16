using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;

namespace PrismMauiApp.Services
{
    public class DisplayRepository : IDisplayRepository
    {
        private readonly ISecureStorage secureStorage;

        public DisplayRepository(ISecureStorage secureStorage)
        {
            this.secureStorage = secureStorage;
        }

        public async Task<ICollection<DisplayConfiguration>> GetDisplayConfigurationsAsync()
        {
            var jsonContent = (await this.secureStorage.GetAsync("DisplayConfigurations")) ?? "[]";
            var displayConfigurations = JsonConvert.DeserializeObject<List<DisplayConfiguration>>(jsonContent);
            return displayConfigurations;
        }

        public async Task AddOrUpdateDisplayConfigurationAsync(DisplayConfiguration displayConfiguration)
        {
            var displayConfigurations = await this.GetDisplayConfigurationsAsync();
            var existingDisplayConfiguration = displayConfigurations.SingleOrDefault(c => c.DisplayName == displayConfiguration.DisplayName);
            if (existingDisplayConfiguration != null)
            {
                displayConfigurations.Remove(existingDisplayConfiguration);
            }

            displayConfigurations.Add(displayConfiguration);

            await this.UpdateDisplayConfigurationsAsync(displayConfigurations);
        }

        private async Task UpdateDisplayConfigurationsAsync(ICollection<DisplayConfiguration> displayConfigurations)
        {
            var jsonContent = JsonConvert.SerializeObject(displayConfigurations);
            await this.secureStorage.SetAsync("DisplayConfigurations", jsonContent);
        }

        public async Task RemoveDisplayConfigurationAsync(DisplayConfiguration displayConfiguration)
        {
            var displayConfigurations = await this.GetDisplayConfigurationsAsync();
            var existingDisplayConfiguration = displayConfigurations.SingleOrDefault(c => c.DisplayName == displayConfiguration.DisplayName);
            if (existingDisplayConfiguration != null)
            {
                displayConfigurations.Remove(existingDisplayConfiguration);
            }

            await this.UpdateDisplayConfigurationsAsync(displayConfigurations);
        }
    }
}
