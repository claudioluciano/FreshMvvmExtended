using Xamarin.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace FreshMvvmExtended
{
    public class FreshTabbedNavigationContainer : Xamarin.Forms.TabbedPage, IFreshNavigationService
    {
        List<Page> _tabs = new List<Page>();
        public IEnumerable<Page> TabbedPages { get { return _tabs; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bottomToolBar">Set TabBar to bottom page on Android</param>
        public FreshTabbedNavigationContainer(bool bottomToolBar = false) : this(FreshConstants.DefaultNavigationServiceName, bottomToolBar)
        {

        }

        public FreshTabbedNavigationContainer(string navigationServiceName, bool bottomToolBar = false)
        {
            if (bottomToolBar)
                this.On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            NavigationServiceName = navigationServiceName;
            RegisterNavigation();
        }

        protected void RegisterNavigation()
        {
            FreshIOC.Container.Register<IFreshNavigationService>(this, NavigationServiceName);
        }

        public virtual Page AddTab<T>(string title, string icon = null, bool IsNavigationPage = false, object data = null) where T : FreshBaseViewModel
        {
            var page = FreshViewModelResolver.ResolvePageModel<T>(data);

            page.GetModel().CurrentNavigationServiceName = NavigationServiceName;

            _tabs.Add(page);

            Page navigationContainer;

            if (IsNavigationPage)
                navigationContainer = CreateContainerPageSafe(page);
            else
                navigationContainer = page;

            navigationContainer.Title = title;

            if (!string.IsNullOrWhiteSpace(icon))
                navigationContainer.Icon = icon;

            Children.Add(navigationContainer);

            return navigationContainer;
        }

        internal Page CreateContainerPageSafe(Page page)
        {
            if (page is NavigationPage || page is MasterDetailPage || page is Xamarin.Forms.TabbedPage)
                return page;

            return CreateContainerPage(page);
        }

        protected virtual Page CreateContainerPage(Page page)
        {
            return new NavigationPage(page);
        }

        public System.Threading.Tasks.Task PushPage(Xamarin.Forms.Page page, FreshBaseViewModel model, bool modal = false, bool animate = true)
        {
            if (modal)
                return this.CurrentPage.Navigation.PushModalAsync(CreateContainerPageSafe(page));
            return this.CurrentPage.Navigation.PushAsync(page);
        }

        public System.Threading.Tasks.Task PopPage(bool modal = false, bool animate = true)
        {
            if (modal)
                return this.CurrentPage.Navigation.PopModalAsync(animate);
            return this.CurrentPage.Navigation.PopAsync(animate);
        }

        public Task PopToRoot(bool animate = true)
        {
            return this.CurrentPage.Navigation.PopToRootAsync(animate);
        }

        public string NavigationServiceName { get; private set; }

        public void NotifyChildrenPageWasPopped()
        {
            foreach (var page in this.Children)
            {
                if (page is NavigationPage)
                    ((NavigationPage)page).NotifyAllChildrenPopped();
            }
        }

        public Task<FreshBaseViewModel> SwitchSelectedRootPageModel<T>() where T : FreshBaseViewModel
        {
            var page = _tabs.FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);

            if (page > -1)
            {
                CurrentPage = this.Children[page];
                var topOfStack = CurrentPage.Navigation.NavigationStack.LastOrDefault();
                if (topOfStack != null)
                    return Task.FromResult(topOfStack.GetModel());

            }
            return null;
        }
    }
}

