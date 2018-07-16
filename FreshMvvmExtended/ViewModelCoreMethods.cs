﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace FreshMvvmExtended
{
    public class ViewModelCoreMethods : IViewModelCoreMethods
    {
        Page _currentPage;
        FreshBaseViewModel _currentPageModel;

        public ViewModelCoreMethods(Page currentPage, FreshBaseViewModel pageModel)
        {
            _currentPage = currentPage;
            _currentPageModel = pageModel;
        }

        public async Task DisplayAlert(string title, string message, string cancel)
        {
            if (_currentPage != null)
                await _currentPage.DisplayAlert(title, message, cancel);
        }

        public async Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            if (_currentPage != null)
                return await _currentPage.DisplayActionSheet(title, cancel, destruction, buttons);
            return null;
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            if (_currentPage != null)
                return await _currentPage.DisplayAlert(title, message, accept, cancel);
            return false;
        }

        public async Task PushPageModel<T>(object data, bool modal = false, bool animate = true) where T : FreshBaseViewModel
        {
            T pageModel = FreshIOC.Container.Resolve<T>();

            await PushPageModel(pageModel, data, modal, animate);
        }

        public async Task PushPageModel<T, TPage>(object data, bool modal = false, bool animate = true) where T : FreshBaseViewModel where TPage : Page
        {
            T pageModel = FreshIOC.Container.Resolve<T>();
            TPage page = FreshIOC.Container.Resolve<TPage>();
            FreshViewModelResolver.BindingPageModel(data, page, pageModel);
            await PushPageModelWithPage(page, pageModel, data, modal, animate);
        }

        public Task PushPageModel(Type pageModelType, bool animate = true)
        {
            return PushPageModel(pageModelType, null, animate);
        }

        public Task PushPageModel(Type pageModelType, object data, bool modal = false, bool animate = true)
        {
            var pageModel = FreshIOC.Container.Resolve(pageModelType) as FreshBaseViewModel;

            return PushPageModel(pageModel, data, modal, animate);
        }

        async Task PushPageModel(FreshBaseViewModel pageModel, object data, bool modal = false, bool animate = true)
        {
            var page = FreshViewModelResolver.ResolvePageModel(data, pageModel);
            await PushPageModelWithPage(page, pageModel, data, modal, animate);
        }

        async Task PushPageModelWithPage(Page page, FreshBaseViewModel pageModel, object data, bool modal = false, bool animate = true)
        {
            pageModel.PreviousPageModel = _currentPageModel; //This is the previous page model because it's push to a new one, and this is current
            pageModel.CurrentNavigationServiceName = _currentPageModel.CurrentNavigationServiceName;

            if (string.IsNullOrWhiteSpace(pageModel.PreviousNavigationServiceName))
                pageModel.PreviousNavigationServiceName = _currentPageModel.PreviousNavigationServiceName;

            if (page is FreshMasterDetailNavigationContainer)
            {
                await this.PushNewNavigationServiceModal((FreshMasterDetailNavigationContainer)page, pageModel, animate);
            }
            else if (page is FreshTabbedNavigationContainer)
            {
                await this.PushNewNavigationServiceModal((FreshTabbedNavigationContainer)page, pageModel, animate);
            }
            else
            {
                IFreshNavigationService rootNavigation = FreshIOC.Container.Resolve<IFreshNavigationService>(_currentPageModel.CurrentNavigationServiceName);

                await rootNavigation.PushPage(page, pageModel, modal, animate);
            }
        }

        public async Task PopPageModel(bool modal = false, bool animate = true)
        {
            string navServiceName = _currentPageModel.CurrentNavigationServiceName;
            if (_currentPageModel.IsModalFirstChild)
            {
                await PopModalNavigationService(animate);
            }
            else
            {
                if (modal)
                    this._currentPageModel.RaisePageWasPopped();

                IFreshNavigationService rootNavigation = FreshIOC.Container.Resolve<IFreshNavigationService>(navServiceName);
                await rootNavigation.PopPage(modal, animate);
            }
        }

        public async Task PopToRoot(bool animate)
        {
            IFreshNavigationService rootNavigation = FreshIOC.Container.Resolve<IFreshNavigationService>(_currentPageModel.CurrentNavigationServiceName);
            await rootNavigation.PopToRoot(animate);
        }

        public async Task PopPageModel(object data, bool modal = false, bool animate = true)
        {
            if (_currentPageModel != null && _currentPageModel.PreviousPageModel != null && data != null)
            {
                _currentPageModel.PreviousPageModel.ReverseInit(data);
            }
            await PopPageModel(modal, animate);
        }

        public Task PushPageModel<T>(bool animate = true) where T : FreshBaseViewModel
        {
            return PushPageModel<T>(null, false, animate);
        }

        public Task PushPageModel<T, TPage>(bool animate = true) where T : FreshBaseViewModel where TPage : Page
        {
            return PushPageModel<T, TPage>(null, animate);
        }

        public Task PushNewNavigationServiceModal(FreshTabbedNavigationContainer tabbedNavigationContainer, FreshBaseViewModel basePageModel = null, bool animate = true)
        {
            var models = tabbedNavigationContainer.TabbedPages.Select(o => o.GetModel()).ToList();
            if (basePageModel != null)
                models.Add(basePageModel);
            return PushNewNavigationServiceModal(tabbedNavigationContainer, models.ToArray(), animate);
        }

        public Task PushNewNavigationServiceModal(FreshMasterDetailNavigationContainer masterDetailContainer, FreshBaseViewModel basePageModel = null, bool animate = true)
        {
            var models = masterDetailContainer.Pages.Select(o =>
               {
                   if (o.Value is NavigationPage)
                       return ((NavigationPage)o.Value).CurrentPage.GetModel();
                   else
                       return o.Value.GetModel();
               }).ToList();

            if (basePageModel != null)
                models.Add(basePageModel);

            return PushNewNavigationServiceModal(masterDetailContainer, models.ToArray(), animate);
        }

        public Task PushNewNavigationServiceModal(IFreshNavigationService newNavigationService, FreshBaseViewModel basePageModels, bool animate = true)
        {
            return PushNewNavigationServiceModal(newNavigationService, new FreshBaseViewModel[] { basePageModels }, animate);
        }

        public async Task PushNewNavigationServiceModal(IFreshNavigationService newNavigationService, FreshBaseViewModel[] basePageModels, bool animate = true)
        {
            var navPage = newNavigationService as Page;
            if (navPage == null)
                throw new Exception("Navigation service is not Page");

            foreach (var pageModel in basePageModels)
            {
                pageModel.CurrentNavigationServiceName = newNavigationService.NavigationServiceName;
                pageModel.PreviousNavigationServiceName = _currentPageModel.CurrentNavigationServiceName;
                pageModel.IsModalFirstChild = true;
            }

            IFreshNavigationService rootNavigation = FreshIOC.Container.Resolve<IFreshNavigationService>(_currentPageModel.CurrentNavigationServiceName);
            await rootNavigation.PushPage(navPage, null, true, animate);
        }

        /// <summary>
        /// Change the navigationContainer
        /// </summary>
        /// <param name="navigationServiceName"></param>
        /// <param name="NotifyContainer">Call the Init for every page on the container</param>
        public void SwitchOutRootNavigation(string navigationServiceName, bool NotifyContainer)
        {
            IFreshNavigationService rootNavigation = FreshIOC.Container.Resolve<IFreshNavigationService>(navigationServiceName);

            if (!(rootNavigation is Page))
                throw new Exception("Navigation service is not a page");

            if (NotifyContainer)
                (rootNavigation as Page).NotifyAllPagesFromContainer();

            Xamarin.Forms.Application.Current.MainPage = rootNavigation as Page;
        }

        public async Task PopModalNavigationService(bool animate = true)
        {
            var currentNavigationService = FreshIOC.Container.Resolve<IFreshNavigationService>(_currentPageModel.CurrentNavigationServiceName);
            currentNavigationService.NotifyChildrenPageWasPopped();

            FreshIOC.Container.Unregister<IFreshNavigationService>(_currentPageModel.CurrentNavigationServiceName);

            var navServiceName = _currentPageModel.PreviousNavigationServiceName;
            IFreshNavigationService rootNavigation = FreshIOC.Container.Resolve<IFreshNavigationService>(navServiceName);
            await rootNavigation.PopPage(animate);
        }

        /// <summary>
        /// This method switches the selected main page, TabbedPage the selected tab or if MasterDetail, works with custom pages also
        /// </summary>
        public Task<FreshBaseViewModel> SwitchSelectedRootPageModel<T>() where T : FreshBaseViewModel
        {
            var currentNavigationService = FreshIOC.Container.Resolve<IFreshNavigationService>(_currentPageModel.CurrentNavigationServiceName);

            return currentNavigationService.SwitchSelectedRootPageModel<T>();
        }

        /// <summary>
        /// This method is used when you want to switch the selected page, 
        /// </summary>
        public Task<FreshBaseViewModel> SwitchSelectedTab<T>() where T : FreshBaseViewModel
        {
            return SwitchSelectedRootPageModel<T>();
        }

        /// <summary>
        /// This method is used when you want to switch the selected page, 
        /// </summary>
        public Task<FreshBaseViewModel> SwitchSelectedMaster<T>() where T : FreshBaseViewModel
        {
            return SwitchSelectedRootPageModel<T>();
        }

        public async Task<string> PushPageModelWithNewNavigation<T>(object data, bool animate = true) where T : FreshBaseViewModel
        {
            var page = FreshViewModelResolver.ResolvePageModel<T>(data);
            var navigationName = Guid.NewGuid().ToString();
            var naviationContainer = new FreshNavigationContainer(page, navigationName);
            await PushNewNavigationServiceModal(naviationContainer, page.GetModel(), animate);
            return navigationName;
        }

        public void BatchBegin()
        {
            _currentPage.BatchBegin();
        }

        public void BatchCommit()
        {
            _currentPage.BatchCommit();
        }

        /// <summary>
        /// Removes current page/pagemodel from navigation
        /// </summary>
        public void RemoveFromNavigation()
        {
            this._currentPageModel.RaisePageWasPopped();
            this._currentPage.Navigation.RemovePage(_currentPage);
        }

        /// <summary>
        /// Removes specific page/pagemodel from navigation
        /// </summary>
        public void RemoveFromNavigation<TPageModel>(bool removeAll = false) where TPageModel : FreshBaseViewModel
        {
            //var pages = this._currentPage.Navigation.Where (o => o is TPageModel);
            foreach (var page in this._currentPage.Navigation.NavigationStack.Reverse().ToList())
            {
                if (page.BindingContext is TPageModel)
                {
                    page.GetModel()?.RaisePageWasPopped();
                    this._currentPage.Navigation.RemovePage(page);
                    if (!removeAll)
                        break;
                }
            }
        }
    }
}
