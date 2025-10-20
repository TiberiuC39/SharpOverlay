using Core.Models;

namespace Core.Events
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
