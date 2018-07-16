using System;

namespace FreshMvvmExtended
{
    public interface IFreshViewModelMapper
    {
        string GetPageTypeName(Type pageModelType);
    }
}

