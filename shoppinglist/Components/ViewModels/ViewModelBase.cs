using System;

namespace shoppinglist.Components.ViewModels;

public abstract class ViewModelBase : IDisposable
{
    public event Action? Changed;

    protected void RaiseChanged() => Changed?.Invoke();

    public virtual void Dispose()
    {
        Changed = null;
    }
}

