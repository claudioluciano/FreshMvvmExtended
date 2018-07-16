using System;
using Xamarin.Forms;

namespace FreshMvvmExtended
{
    public static class FreshViewModelResolver
    {
        public static IFreshViewModelMapper PageModelMapper { get; set; } = new FreshViewModelMapper();

        public static Page ResolvePageModel<T> () where T : FreshBaseViewModel
        {
            return ResolvePageModel<T> (null);
        }

        public static Page ResolvePageModel<T> (object initData) where T : FreshBaseViewModel
        {
            var pageModel = FreshIOC.Container.Resolve<T> ();

            return ResolvePageModel<T> (initData, pageModel);
        }

        public static Page ResolvePageModel<T> (object data, T pageModel) where T : FreshBaseViewModel
        {
            var type = pageModel.GetType ();
            return ResolvePageModel (type, data, pageModel);
        }

        public static Page ResolvePageModel (Type type, object data) 
        {
            var pageModel = FreshIOC.Container.Resolve (type) as FreshBaseViewModel;
            return ResolvePageModel (type, data, pageModel);
        }

        public static Page ResolvePageModel (Type type, object data, FreshBaseViewModel pageModel)
        {
            var name = PageModelMapper.GetPageTypeName (type);
            var pageType = Type.GetType (name);
            if (pageType == null)
                throw new Exception (name + " not found");

            var page = (Page)FreshIOC.Container.Resolve (pageType);

            BindingPageModel(data, page, pageModel);

            return page;
        }

        public static Page BindingPageModel(object data, Page targetPage, FreshBaseViewModel pageModel)
        {
            pageModel.WireEvents (targetPage);
            pageModel.CurrentPage = targetPage;
            pageModel.CoreMethods = new ViewModelCoreMethods (targetPage, pageModel);
            pageModel.Init (data);
            targetPage.BindingContext = pageModel;
            return targetPage;
        }            
    }
}

