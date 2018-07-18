using System;
using Xamarin.Forms;

namespace FreshMvvmExtended
{
    public static class FreshViewModelResolver
    {
        public static IFreshViewModelMapper ViewModelMapper { get; set; } = new FreshViewModelMapper();

        public static Page ResolveViewModel<T> () where T : FreshBaseViewModel
        {
            return ResolveViewModel<T> (null);
        }

        public static Page ResolveViewModel<T> (object initData) where T : FreshBaseViewModel
        {
            var pageModel = FreshIOC.Container.Resolve<T> ();

            return ResolveViewModel<T> (initData, pageModel);
        }

        public static Page ResolveViewModel<T> (object data, T pageModel) where T : FreshBaseViewModel
        {
            var type = pageModel.GetType ();
            return ResolveViewModel (type, data, pageModel);
        }

        public static Page ResolveViewModel (Type type, object data) 
        {
            var pageModel = FreshIOC.Container.Resolve (type) as FreshBaseViewModel;
            return ResolveViewModel (type, data, pageModel);
        }

        public static Page ResolveViewModel (Type type, object data, FreshBaseViewModel pageModel)
        {
            var name = ViewModelMapper.GetPageTypeName (type);
            var pageType = Type.GetType (name);
            if (pageType == null)
                throw new Exception (name + " not found");

            var page = (Page)FreshIOC.Container.Resolve (pageType);

            BindingViewModel(data, page, pageModel);

            return page;
        }

        public static Page BindingViewModel(object data, Page targetPage, FreshBaseViewModel pageModel)
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

