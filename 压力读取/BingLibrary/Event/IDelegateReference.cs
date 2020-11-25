using System;

namespace BingLibrary.hjb.events
{
    public interface IDelegateReference
    {
        Delegate Target { get; }
    }
}