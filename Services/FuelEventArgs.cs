using SharpOverlay.Models;
using System;

namespace SharpOverlay.Services
{
    public class FuelEventArgs : EventArgs
    {
        public FuelEventArgs(FuelViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public FuelViewModel ViewModel { get; }
    }
}