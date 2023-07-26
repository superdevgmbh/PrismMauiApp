using FluentAssertions;
using Moq;
using Moq.AutoMock;
using PrismMauiApp.Services;
using PrismMauiApp.ViewModels;

namespace PrismMauiApp.Tests.ViewModels
{
    public class MainViewModelTests
    {
        private readonly AutoMocker autoMocker;

        public MainViewModelTests()
        {
            this.autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task ShouldInitializeAsync()
        {
            // Arrange
            var displayRepositoryMock = this.autoMocker.GetMock<IDisplayRepository>();
            displayRepositoryMock.Setup(n => n.GetDisplayConfigurationsAsync())
                .ReturnsAsync(new List<DisplayConfiguration> { new DisplayConfiguration() });

            var viewModel = this.autoMocker.CreateInstance<MainViewModel>();

            // Act
            await ((IInitializeAsync)viewModel).InitializeAsync(null);

            // Assert
            viewModel.IsInitialized.Should().BeTrue();

            displayRepositoryMock.Verify(n => n.GetDisplayConfigurationsAsync(), Times.Once);
            displayRepositoryMock.VerifyNoOtherCalls();
        }
    }
}
