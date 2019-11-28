using FlyApp.Forms.Views;
using Xamarin.Forms;

namespace FlyApp.Forms
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("ItemsPage", typeof(ItemsPage));
        }
    }
}