using Microsoft.UI.Xaml;

// WinUI app.

namespace TimeHelper.WinUI
{
    /// <summary>
    /// Windows app.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// App start.
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
