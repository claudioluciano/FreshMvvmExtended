using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FreshMvvmExtended.Test.Mocks
{
    public class MockContentPage : ContentPage
    {
        public MockContentPage(FreshBaseViewModel model)
        {
            this.BindingContext = model;
        }
    }
}
