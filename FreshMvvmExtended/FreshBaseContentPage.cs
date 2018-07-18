﻿using Xamarin.Forms;
using System.Collections.Specialized;

namespace FreshMvvmExtended
{
    public class FreshBaseContentPage : ContentPage
    {
        public FreshBaseContentPage ()
        {
        }

        protected override void OnBindingContextChanged ()
        {
            base.OnBindingContextChanged ();

            if (BindingContext is FreshBaseViewModel pageModel && pageModel.ToolbarItems != null && pageModel.ToolbarItems.Count > 0)
            {

                pageModel.ToolbarItems.CollectionChanged += ViewModel_ToolbarItems_CollectionChanged;

                foreach (var toolBarItem in pageModel.ToolbarItems)
                {
                    if (!(this.ToolbarItems.Contains(toolBarItem)))
                    {
                        this.ToolbarItems.Add(toolBarItem);
                    }
                }
            }

        }

        void ViewModel_ToolbarItems_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (ToolbarItem toolBarItem in e.NewItems) {
                if (!(this.ToolbarItems.Contains (toolBarItem))) {
                    this.ToolbarItems.Add (toolBarItem);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (ToolbarItem toolBarItem in e.OldItems) {
                    if (!(this.ToolbarItems.Contains (toolBarItem))) {
                        this.ToolbarItems.Add (toolBarItem);
                    }
                }
            }                
        }
    }
}

