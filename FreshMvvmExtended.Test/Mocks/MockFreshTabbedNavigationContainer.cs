using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FreshMvvmExtended.Test.Mocks
{
    public class MockFreshTabbedNavigationContainer : Xamarin.Forms.TabbedPage, IFreshNavigationService
    {
        Dictionary<Page, (Color BarBackgroundColor, Color TextColor)> _tabs = new Dictionary<Page, (Color BarBackgroundColor, Color TextColor)>();

        public string NavigationServiceName { get; }

        public void NotifyChildrenPageWasPopped()
        {
            throw new NotImplementedException();
        }

        public Task PopPage(bool modal = false, bool animate = true)
        {
            throw new NotImplementedException();
        }

        public Task PopToRoot(bool animate = true)
        {
            throw new NotImplementedException();
        }

        public Task PushPage(Page page, FreshBaseViewModel model, bool modal = false, bool animate = true)
        {
            throw new NotImplementedException();
        }

        public Task<FreshBaseViewModel> SwitchSelectedRootViewModel<T>() where T : FreshBaseViewModel
        {
            var page = _tabs.Keys.ToList().FindIndex(o => o.GetModel().GetType().FullName == typeof(T).FullName);

            if (page > -1)
            {
                CurrentPage = this.Children[page];
                var topOfStack = CurrentPage.Navigation.NavigationStack.LastOrDefault();
                if (topOfStack != null)
                    return Task.FromResult(topOfStack.GetModel());
            }
            return null;
        }

        public virtual Page AddTab(Page page, string title, string icon = null, bool IsNavigationPage = false, object data = null)
        {
            page.GetModel().CurrentNavigationServiceName = NavigationServiceName;

            _tabs.Add(page, (Color.White, Color.Black));

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
        protected override void OnCurrentPageChanged()
        {
            var color = _tabs.FirstOrDefault(x => x.Key.GetType().FullName == this.CurrentPage.GetType().FullName).Value;

            this.BarBackgroundColor = color.BarBackgroundColor;
            this.BarTextColor = color.TextColor;

            base.OnCurrentPageChanged();
        }

    }
}
