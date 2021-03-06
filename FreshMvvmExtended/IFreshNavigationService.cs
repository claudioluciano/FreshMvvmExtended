﻿using Xamarin.Forms;
using System.Threading.Tasks;

namespace FreshMvvmExtended
{
	public interface IFreshNavigationService
	{
		Task PopToRoot(bool animate = true);

		Task PushPage (Page page, FreshBaseViewModel model, bool modal = false, bool animate = true);

		Task PopPage (bool modal = false, bool animate = true);

		/// <summary>
		/// This method switches the selected main page, TabbedPage the selected tab or if MasterDetail, works with custom pages also
		/// </summary>
		/// <returns>The BageViewModel, allows you to PopToRoot, Pass Data</returns>
		/// <param name="newSelected">The pagemodel of the root you want to change</param>
		Task<FreshBaseViewModel> SwitchSelectedRootViewModel<T>() where T : FreshBaseViewModel;

		void NotifyChildrenPageWasPopped();

		string NavigationServiceName { get; }
	}
}

