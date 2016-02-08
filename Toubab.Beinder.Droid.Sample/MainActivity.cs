using Android.App;
using Android.Widget;
using Android.OS;
using Toubab.Beinder.Tools;
using System.Threading.Tasks;
using System;

namespace Toubab.Beinder.Droid.Sample
{
    [Activity(Label = "Sample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button clickMeButton = FindViewById<Button>(Resource.Id.hello_world_button);
            
            clickMeButton.Click += async delegate
            {
                clickMeButton.Text = string.Format("{0} clicks!", count++);
                Activity emptyActivity =
                    await this.StartActivityWithCallbacksAsync<EmptyActivity>(
                        created: (activity, sis) =>
                        {
                            if (sis == null)
                            {
                                Toast.MakeText(this, "Created for the first time!", ToastLength.Short).Show();
                            }
                            else
                            {
                                Toast.MakeText(this, "Created again after being deflated!", ToastLength.Short).Show();
                            }
                        },
                        started: activity => Toast.MakeText(this, "Activity started!", ToastLength.Short).Show(),
                        resumed: activity => Toast.MakeText(this, "Activity resumed!", ToastLength.Short).Show(),
                        paused: activity => Toast.MakeText(this, "Activity paused", ToastLength.Short).Show(),
                        stopped: activity => Toast.MakeText(this, "Activity stopped", ToastLength.Short).Show(),
                        destroyed: activity => Toast.MakeText(this, "Activity destroyed", ToastLength.Short).Show(),
                        saveInstanceState: (activity, bundle) => Toast.MakeText(this, "Activity saved instance state", ToastLength.Short).Show()
                    );

                Toast.MakeText(this, "Got activity: " + emptyActivity.Title, ToastLength.Short).Show();
            };


            Button goToSimpleActivityButton = FindViewById<Button>(Resource.Id.button_simple_activity);

            Binder binder = new Binder();
            binder.Scanner.Add(new AndroidScanner());
            IBindings bindings;
            var vm = new SimpleViewModel();
            var tcs = new TaskCompletionSource<int>();

            goToSimpleActivityButton.Click += async delegate
            {
                this.StartActivityWithCallbacks<SimpleActivity>(
                    started: async activity =>
                    {
                        try
                        {
                            bindings = await binder.Bind(vm, activity);
                            tcs.SetResult(0);
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }
                    }
                );
            };
        }
    }

    class SimpleViewModel : NotifyPropertyChanged
    {
        int _clickCount = 0;

        public void MyButtonClick()
        {
            _clickCount++;
            MyButtonText = string.Format("Clicked {0} times!", _clickCount);
        }

        string _myButtonText;

        public string MyButtonText
        {
            get { return _myButtonText; }
            set { SetProperty(ref _myButtonText, value); }
        }

    }
}


