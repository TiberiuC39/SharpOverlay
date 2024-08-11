using System;
using System.Windows;

namespace SharpOverlay.Models
{
    public class Overlay
    {
        public Type Type { get; set; }
        public Window? Window { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsOpen { get; set; }

        public Overlay(Type type, bool isEnabled, bool isOpen)
        {
            Type = type;
            Window = null;
            IsEnabled = isEnabled;
            IsOpen = isOpen;
        }
    }
}
