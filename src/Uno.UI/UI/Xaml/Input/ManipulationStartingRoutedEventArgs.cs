using Windows.Devices.Input;
using Windows.UI.Input;
using Uno.UI.Xaml.Input;

namespace Windows.UI.Xaml.Input
{
	public partial class ManipulationStartingRoutedEventArgs : RoutedEventArgs, IHandleableRoutedEventArgs
	{
		private readonly ManipulationStartingEventArgs _args;
		private ManipulationModes _mode;

		public ManipulationStartingRoutedEventArgs() { }

		internal ManipulationStartingRoutedEventArgs(UIElement container, ManipulationStartingEventArgs args)
			: base(container)
		{
			Container = container;

			_args = args;

			// We should convert back the enum from the args.Settings, but its the same value of the container.ManipulationMode
			// so we use the easiest path!
			_mode = container.ManipulationMode;

			Pointer = args.Pointer;
		}

		/// <summary>
		/// Gets identifier of the first pointer for which a manipulation is considered
		/// </summary>
		internal PointerIdentifier Pointer { get; }

		public bool Handled { get; set; }

		public UIElement Container { get; set; }

		public ManipulationModes Mode
		{
			get => _mode;
			set
			{
				_mode = value;
				if (_args != null)
				{
					_args.Settings = value.ToGestureSettings();
				}
			}
		}
	}
}
