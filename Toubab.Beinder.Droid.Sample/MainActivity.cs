using Android.App;
using Android.Widget;
using Android.OS;

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
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);
            
            button.Click += async delegate
            {
                button.Text = string.Format("{0} clicks!", count++);
                Activity emptyActivity =
                    await this.StartActivityWithCallbacksAsync<EmptyActivity>(
                        created: (activity, sis) => {
                            if (sis == null) {
                                Toast.MakeText(this, "Created for the first time!", ToastLength.Short).Show();
                            } else {
                                Toast.MakeText(this, "Created again after being deflated!", ToastLength.Short).Show();
                            }
                        },
                        started:           activity => Toast.MakeText(this, "Activity started!", ToastLength.Short).Show(),
                        resumed:           activity => Toast.MakeText(this, "Activity resumed!", ToastLength.Short).Show(),
                        paused:            activity => Toast.MakeText(this, "Activity paused", ToastLength.Short).Show(),
                        stopped:           activity => Toast.MakeText(this, "Activity stopped", ToastLength.Short).Show(),
                        destroyed:         activity => Toast.MakeText(this, "Activity destroyed", ToastLength.Short).Show(),
                        saveInstanceState: (activity, bundle) => Toast.MakeText(this, "Activity saved instance state", ToastLength.Short).Show()
                    );

                Toast.MakeText(this, "Got activity: " + emptyActivity.Title, ToastLength.Short).Show();
            };


        }
    }
}


