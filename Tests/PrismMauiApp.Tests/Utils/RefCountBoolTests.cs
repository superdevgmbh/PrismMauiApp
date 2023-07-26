using Moq.AutoMock;
using PrismMauiApp.Utils;
using PrismMauiApp.ViewModels;

namespace PrismMauiApp.Tests.Utils
{
    public class RefCountBoolTests
    {
        private readonly AutoMocker autoMocker;

        public RefCountBoolTests()
        {
            this.autoMocker = new AutoMocker();
        }

        [Fact]
        public void IsBusyTest_TrueTrueFalse()
        {
            // Arrange
            var propertyChangedList = new List<string>();
            var viewModel = this.autoMocker.CreateInstance<NotImplementedViewModel>();
            viewModel.PropertyChanged += (sender, args) => { propertyChangedList.Add(args.PropertyName); };

            // Act
            viewModel.IsScannerBusy = true;
            viewModel.IsScannerBusy = true;
            viewModel.IsScannerBusy = false;

            // Assert
            Assert.True(viewModel.IsScannerBusy);
            Assert.Equal(1, propertyChangedList.Count(p => p == "IsScannerBusy"));
        }

        [Fact]
        public void IsBusyTest_TrueTrueFalseFalse()
        {
            // Arrange
            var propertyChangedList = new List<string>();
            var viewModel = this.autoMocker.CreateInstance<NotImplementedViewModel>();
            viewModel.PropertyChanged += (sender, args) => { propertyChangedList.Add(args.PropertyName); };

            // Act
            viewModel.IsScannerBusy = true;
            viewModel.IsScannerBusy = true;
            viewModel.IsScannerBusy = false;
            viewModel.IsScannerBusy = false;

            // Assert
            Assert.False(viewModel.IsScannerBusy);
            Assert.Equal(2, propertyChangedList.Count(p => p == "IsScannerBusy"));
        }

        [Fact]
        public void IsBusyTest_FalseFalseTrue()
        {
            // Arrange
            var propertyChangedList = new List<string>();
            var viewModel = this.autoMocker.CreateInstance<NotImplementedViewModel>();
            viewModel.PropertyChanged += (sender, args) => { propertyChangedList.Add(args.PropertyName); };

            // Act
            viewModel.IsScannerBusy = false;
            viewModel.IsScannerBusy = false;
            viewModel.IsScannerBusy = true;

            // Assert
            Assert.False(viewModel.IsScannerBusy);
            Assert.Equal(0, propertyChangedList.Count(p => p == "IsScannerBusy"));
        }
    }

    public class NotImplementedViewModel : ViewModelBase
    {
        private readonly RefCountBool isScannerBusy = new RefCountBool();

        public bool IsScannerBusy
        {
            get => this.isScannerBusy;
            set
            {
                if (this.SetProperty(this.isScannerBusy, value))
                {

                }
            }
        }
    }
}
