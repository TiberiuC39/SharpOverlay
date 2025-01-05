using SharpOverlay.Models;
using System;

namespace SharpOverlay.Services.FuelServices
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