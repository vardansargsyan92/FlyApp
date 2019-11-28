using FlyApp.Core;
using FlyApp.Forms.Services;
using FlyApp.ViewModels;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Xamarin.Forms;

namespace FlyApp.Forms
{
    public partial class App
    {
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var container = containerRegistry.GetContainer();
            container
                .RegisterCoreDependencies()
                .RegisterViewModelsDependencies();
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}