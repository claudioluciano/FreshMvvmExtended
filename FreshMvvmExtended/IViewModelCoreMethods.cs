using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FreshMvvmExtended
{
    public interface IViewModelCoreMethods
    {
        Task DisplayAlert(string title, string message, string cancel);

        Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons);

        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);

        Task PushPageModel<T>(object data, bool modal = false, bool animate = true) where T : FreshBaseViewModel;

        Task PushPageModel<T, TPage>(object data, bool modal = false, bool animate = true) where T : FreshBaseViewModel where TPage : Page;

        Task PopPageModel(bool modal = false, bool animate = true);

        Task PopPageModel(object data, bool modal = false, bool animate = true);

        Task PushPageModel<T>(bool animate = true) where T : FreshBaseViewModel;

        Task PushPageModel<T, TPage>(bool animate = true) where T : FreshBaseViewModel where TPage : Page;

        Task PushPageModel(Type pageModelType, bool animate = true);

        /// <summary>
        /// Removes current page/pagemodel from navigation
        /// </summary>
        void RemoveFromNavigation();

        /// <summary>
        /// Removes specific page/pagemodel from navigation
        /// </summary>
        /// <param name="removeAll">Will remove all, otherwise it will just remove first on from the top of the stack</param>
        /// <typeparam name="TPageModel">The 1st type parameter.</typeparam>
        void RemoveFromNavigation<TPageModel>(bool removeAll = false) where TPageModel : FreshBaseViewModel;

        /// <summary>
        /// This method pushes a new PageModel modally with a new NavigationContainer
        /// </summary>
        /// <returns>Returns the name of the new service</returns>
        Task<string> PushPageModelWithNewNavigation<T>(object data, bool animate = true) where T : FreshBaseViewModel;

        Task PushNewNavigationServiceModal(IFreshNavigationService newNavigationService, FreshBaseViewModel[] basePageModels, bool animate = true);

        Task PushNewNavigationServiceModal(FreshTabbedNavigationContainer tabbedNavigationContainer, FreshBaseViewModel basePageModel = null, bool animate = true);

        Task PushNewNavigationServiceModal(FreshMasterDetailNavigationContainer masterDetailContainer, FreshBaseViewModel basePageModel = null, bool animate = true);

        Task PushNewNavigationServiceModal(IFreshNavigationService newNavigationService, FreshBaseViewModel basePageModels, bool animate = true);

        Task PopModalNavigationService(bool animate = true);

        void SwitchOutRootNavigation(string navigationServiceName, bool NotifyContainer);

        /// <summary>
        /// This method switches the selected main page, TabbedPage the selected tab or if MasterDetail, works with custom pages also
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelected">The pagemodel of the root you want to change</param>
        Task<FreshBaseViewModel> SwitchSelectedRootPageModel<T>() where T : FreshBaseViewModel;

        /// <summary>
        /// This method is used when you want to switch the selected page, 
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelectedTab">The pagemodel of the root you want to change</param>
        Task<FreshBaseViewModel> SwitchSelectedTab<T>() where T : FreshBaseViewModel;

        /// <summary>
        /// This method is used when you want to switch the selected page, 
        /// </summary>
        /// <returns>The BagePageModel, allows you to PopToRoot, Pass Data</returns>
        /// <param name="newSelectedMaster">The pagemodel of the root you want to change</param>
        Task<FreshBaseViewModel> SwitchSelectedMaster<T>() where T : FreshBaseViewModel;

        Task PopToRoot(bool animate);

        void BatchBegin();

        void BatchCommit();
    }
}

