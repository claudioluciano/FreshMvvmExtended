using Xamarin.Forms;

namespace FreshMvvmExtended
{
    public static class PageExtensions
    {
        public static FreshBaseViewModel GetModel(this Page page)
        {
            var pageModel = page.BindingContext as FreshBaseViewModel;
            return pageModel;
        }

        public static void NotifyAllChildrenPopped(this NavigationPage navigationPage)
        {
            foreach (var page in navigationPage.Navigation.ModalStack)
            {
                var pageModel = page.GetModel();
                if (pageModel != null)
                    pageModel.RaisePageWasPopped();
            }

            foreach (var page in navigationPage.Navigation.NavigationStack)
            {
                var pageModel = page.GetModel();
                if (pageModel != null)
                    pageModel.RaisePageWasPopped();
            }
        }

        public static void NotifyAllPagesFromContainer(this Page page)
        {
            System.Collections.Generic.List<Page> pages = new System.Collections.Generic.List<Page>();
            if (page is NavigationPage)
                pages.AddRange((page as NavigationPage).Navigation.NavigationStack);
            else if (page is MasterDetailPage)
                pages.AddRange((page as MasterDetailPage).Detail.Navigation.NavigationStack);
            else if (page is TabbedPage)
                pages.AddRange((page as TabbedPage).Children);


            for (int i = 0; i < pages.Count; i++)
                pages[i].GetModel().Init(null);
        }
    }
}

