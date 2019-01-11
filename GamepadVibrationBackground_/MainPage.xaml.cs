using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Gaming.Input;
using Windows.System.Threading;

using System.Threading.Tasks;
using System.Threading;
using Windows.ApplicationModel.ExtendedExecution;

namespace GamepadVibrationBackground_
{
    public sealed partial class MainPage : Page
    {
        ExtendedExecutionSession session = null;
        ThreadPoolTimer timer;
        Gamepad gamepad = null;
        GamepadVibration vibration;

        public MainPage()
        {
            this.InitializeComponent();
            Gamepad.GamepadAdded += On_GamepadAdded;
        }

        async private void On_Loaded(object sender, RoutedEventArgs args)
        {
            await PreventSuspend();
            if (session == null)
                throw new Exception("ExtendedExecution not set");
        }

        async private Task PreventSuspend()
        {
            ExtendedExecutionSession newSession = new ExtendedExecutionSession
            {
                Reason = ExtendedExecutionReason.Unspecified
            };

            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();

            if (result != ExtendedExecutionResult.Allowed)
                throw new Exception("ExtendedExecution not allowed");

            session = newSession;
        }

        private void ImpulsePulseTest(object state)
        {
            vibration.LeftMotor = 0.3;
            gamepad.Vibration = vibration;
            Task.Delay(System.TimeSpan.FromMilliseconds(50)).Wait();
            vibration.LeftMotor = 0;
            gamepad.Vibration = vibration;
            Task.Delay(System.TimeSpan.FromMilliseconds(50)).Wait();
        }

        private void On_GamepadAdded(object sender, Gamepad g)
        {
            gamepad = Gamepad.Gamepads?.First();
            vibration = new GamepadVibration();
            timer = ThreadPoolTimer.CreatePeriodicTimer(ImpulsePulseTest, TimeSpan.FromMilliseconds(1000));
        }
    }
}
