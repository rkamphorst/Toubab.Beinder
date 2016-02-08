using Android.App;
using Android.OS;
using Android.Widget;

namespace Toubab.Beinder.Droid.Sample
{
    [Activity(Label = "My Second Activity")]            
    public class EmptyActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            // Set our view from the "second" layout resource
            //SetContentView(Resource.Layout.Second);
            Toast.MakeText(this, "OnCreate done", ToastLength.Short).Show();
        }
    }
}

