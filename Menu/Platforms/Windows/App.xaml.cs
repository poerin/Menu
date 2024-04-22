using Microsoft.Maui.Platform;

namespace Menu.WinUI
{

    public partial class App : MauiWinUIApplication
    {

        public App()
        {
            InitializeComponent();

            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handel, view) =>
            {
                handel.PlatformView.UpdateSize(new Window() { Height = 800, Width = 450 });
            });

        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    }

}
