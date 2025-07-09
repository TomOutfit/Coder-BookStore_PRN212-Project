using System;

namespace PresentationLayer.ViewModels
{
    public static class DataChangeNotifier
    {
        public static event Action? DataChanged;
        public static void NotifyDataChanged() => DataChanged?.Invoke();
    }
} 