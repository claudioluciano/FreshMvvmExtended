using FreshMvvmExtended.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreshMvvmExtended.Test
{
    [TestClass]
    public class TabbedTest
    {
        [TestMethod]
        public void Switch_select_tab()
        {
            var tabContainer = new MockFreshTabbedNavigationContainer();

            var vm1 = new OneViewModel();
            var p1 = new MockContentPage(vm1);
            vm1.CoreMethods = new ViewModelCoreMethods(p1, vm1);

            var vm2 = new OneViewModel();
            var p2 = new MockContentPage(vm2);
            vm2.CoreMethods = new ViewModelCoreMethods(p2, vm2);

            tabContainer.AddTab(p1, "One");
            tabContainer.AddTab(new MockContentPage(new TwoViewModel()), "Two");
            tabContainer.AddTab(new MockContentPage(new ThreeViewModel()), "Three");
            tabContainer.AddTab(p2, "Four");

            tabContainer.CurrentPage = p2;

            var viewModel = p2.GetModel();

            var c = viewModel.CoreMethods.SwitchSelectedTab<OneViewModel>();

            Assert.IsNotNull(c);
        }
    }
}
