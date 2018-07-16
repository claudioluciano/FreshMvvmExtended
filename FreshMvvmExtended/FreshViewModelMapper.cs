using System;

namespace FreshMvvmExtended
{
    public class FreshViewModelMapper : IFreshViewModelMapper
    {
        public string GetPageTypeName(Type pageModelType)
        {
            return pageModelType.AssemblyQualifiedName
                .Replace ("ViewModel", "View");
        }
    }
}

