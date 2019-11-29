namespace FlyApp.ViewModels.Base.Implementation
{
    public class MenuItemViewModel : IMenuItemViewModel
    {
        public MenuItemViewModel(string page, string title)
        {
            Page = page;
            Title = title;
        }

        public string Page { get; set; }
        public string Title { get; set; }
    }
}