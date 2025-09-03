using FluentAssertions;
using PrismMauiApp.ViewModels;

namespace PrismMauiApp.Tests.ViewModels
{
    public class ViewModelBaseTests
    {
        [Fact]
        public void ShouldInitialize()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            ((IInitialize)viewModel).Initialize(null);

            // Assert
            viewModel.IsInitialized.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldInitializeAsync()
        {
            // Arrange
            var viewModel = new TestViewModel();

            // Act
            await ((IInitializeAsync)viewModel).InitializeAsync(null);

            // Assert
            viewModel.IsInitialized.Should().BeTrue();
        }

        [Fact]
        public void ShouldToggleIsBusy()
        {
            // Arrange
            var busyCount = 0;
            var viewModel = new TestViewModel(busyDelegate: () => { busyCount++; });

            // Act
            viewModel.Run();

            // Assert
            busyCount.Should().Be(2);
        }

        public class TestViewModel : ViewModelBase
        {
            private readonly Action busyDelegate;

            public TestViewModel(Action busyDelegate = null)
            {
                this.busyDelegate = busyDelegate;
            }

            public void Run()
            {
                this.IsBusy = true;
                this.IsBusy = false;
            }

            protected override void OnBusyChanged(bool isBusy)
            {
                this.busyDelegate?.Invoke();
            }
        }
    }
}
