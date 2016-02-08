namespace Toubab.Beinder.Droid.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using Android.OS;

    public static class StartActivityExtensions
    {

        /// <summary>
        /// Start an activity with callbacks, and wait until it is started.
        /// </summary>
        /// <remarks>
        /// Tye <typeparam name="T"/> type is used to create a type object, which is passed on to 
        /// the overload with a type parameter.
        /// </remarks>
        /// <seealso cref="StartActivityWithCallbacksAsync(Context, Type, Action{Activity, Bundle}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity, Bundle})"/>.
        public static async Task<T> StartActivityWithCallbacksAsync<T>(this Context context,
                                                          Action<Activity, Bundle> created = null,
                                                          Action<Activity> started = null,
                                                          Action<Activity> resumed = null,
                                                          Action<Activity> paused = null,
                                                          Action<Activity> stopped = null,
                                                          Action<Activity> destroyed = null,
                                                          Action<Activity, Bundle> saveInstanceState = null
        ) where T : Activity
        {
            return (await StartActivityWithCallbacksAsync(context, typeof(T),
                created, started, resumed, paused, stopped, destroyed, saveInstanceState)) as T;
        }

        /// <summary>
        /// Start an activity with callbacks, and wait until it is started.
        /// </summary>
        /// <remarks>
        /// The <paramref name="type"/> argument is used to construct an <see cref="Intent"/>.
        /// </remarks>
        /// <seealso cref="StartActivityWithCallbacksAsync(Context, Intent, Action{Activity, Bundle}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity, Bundle})"/>.
        public static async Task<Activity> StartActivityWithCallbacksAsync(this Context context, Type type,
                                                              Action<Activity, Bundle> created = null,
                                                              Action<Activity> started = null,
                                                              Action<Activity> resumed = null,
                                                              Action<Activity> paused = null,
                                                              Action<Activity> stopped = null,
                                                              Action<Activity> destroyed = null,
                                                              Action<Activity, Bundle> saveInstanceState = null
        )
        {
            return await StartActivityWithCallbacksAsync(context, new Intent(context, type),
                created, started, resumed, paused, stopped, destroyed, saveInstanceState);
        }

        /// <summary>
        /// Start an activity with callbacks, and wait until it is started.
        /// </summary>
        /// <seealso cref="StartActivityWithCallbacksAsync(Context, Intent, Bundle, Action{Activity, Bundle}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity, Bundle})"/>.
        public static async Task<Activity> StartActivityWithCallbacksAsync(this Context context, Intent intent,
                                                              Action<Activity, Bundle> created = null,
                                                              Action<Activity> started = null,
                                                              Action<Activity> resumed = null,
                                                              Action<Activity> paused = null,
                                                              Action<Activity> stopped = null,
                                                              Action<Activity> destroyed = null,
                                                              Action<Activity, Bundle> saveInstanceState = null)
        {
            return await StartActivityWithCallbacksAsync(context, intent, (Bundle)null, 
                created, started, resumed, paused, stopped, destroyed, saveInstanceState
            );
        }

        /// <summary>
        /// Start an activity with callbacks, and wait until the activity exists
        /// </summary>
        /// <remarks>
        /// Starts an activity and waits until it can return an <see cref="Activity"/> object.
        /// 
        /// Makes use of <see cref="StartActivityWithCallbacks(Context, Intent, Bundle, Action{Activity, Bundle}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity, Bundle})"/>;
        /// the parameters are described there.
        /// </remarks>
        public static Task<Activity> StartActivityWithCallbacksAsync(this Context context, Intent intent, Bundle options,
                                                                     Action<Activity, Bundle> created = null,
                                                                     Action<Activity> started = null,
                                                                     Action<Activity> resumed = null,
                                                                     Action<Activity> paused = null,
                                                                     Action<Activity> stopped = null,
                                                                     Action<Activity> destroyed = null,
                                                                     Action<Activity, Bundle> saveInstanceState = null
        )
        {
            var tcs = new TaskCompletionSource<Activity>();
            try
            {
                StartActivityWithCallbacks(context, intent, options,
                    (activity, bundle) =>
                    {
                        try
                        {
                            created(activity, bundle);  
                            tcs.TrySetResult(activity);
                        }
                        catch (Exception ex)
                        {
                            if (bundle == null)
                                tcs.TrySetException(ex);
                        }
                    },
                    activity =>
                    {
                        try
                        {
                            started(activity);
                            tcs.TrySetResult(activity);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    },
                    activity =>
                    {
                        try
                        {
                            resumed(activity);
                            tcs.TrySetResult(activity);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    },
                    activity =>
                    {
                        try
                        {
                            paused(activity);
                            tcs.TrySetResult(activity);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    },
                    activity =>
                    {
                        try
                        {
                            stopped(activity);
                            tcs.TrySetResult(activity);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    },
                    activity =>
                    {
                        try
                        {
                            destroyed(activity);
                            tcs.TrySetResult(activity);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    },
                    (activity, bundle) =>
                    {
                        try
                        {
                            saveInstanceState(activity, bundle);
                            tcs.TrySetResult(activity);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }    

            return tcs.Task;
        }

        /// <summary>
        /// Start an activity with callbacks
        /// </summary>
        /// <remarks>
        /// This method is a utility method for calling <see cref="StartActivityWithCallbacks(Context, Type, Action{Activity, Bundle}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity, Bundle})"/>.
        /// 
        /// The <typeparam name="T"/> type argument is used to supply a type argument.
        /// </remarks>
        public static void StartActivityWithCallbacks<T>(this Context context,
            Action<Activity, Bundle> created = null,
            Action<Activity> started = null,
            Action<Activity> resumed = null,
            Action<Activity> paused = null,
            Action<Activity> stopped = null,
            Action<Activity> destroyed = null,
            Action<Activity, Bundle> saveInstanceState = null
        ) where T : Activity
        {
            StartActivityWithCallbacks(context, new Intent(context, typeof(T)),
                created, started, resumed, paused, stopped, destroyed, saveInstanceState);
        }

        /// <summary>
        /// Start an activity with callbacks
        /// </summary>
        /// <remarks>
        /// This method is a utility method for calling <see cref="StartActivityWithCallbacks(Context, Intent, Action{Activity, Bundle}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity, Bundle})"/>.
        /// 
        /// The <paramref name="type"/> argument is used to create an <see cref="Intent"/>.
        /// </remarks>
        public static void StartActivityWithCallbacks(this Context context, Type type,
            Action<Activity, Bundle> created = null,
            Action<Activity> started = null,
            Action<Activity> resumed = null,
            Action<Activity> paused = null,
            Action<Activity> stopped = null,
            Action<Activity> destroyed = null,
            Action<Activity, Bundle> saveInstanceState = null
        )
        {
            StartActivityWithCallbacks(context, new Intent(context, type), null, 
                created, started, resumed, paused, stopped, destroyed, saveInstanceState);
        }

        /// <summary>
        /// Start an activity with callbacks
        /// </summary>
        /// <remarks>
        /// This method is a utility method for calling <see cref="StartActivityWithCallbacks(Context, Intent, Bundle, Action{Activity, Bundle}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity}, Action{Activity, Bundle})"/>
        /// without the "options" argument.
        /// </remarks>
        public static void StartActivityWithCallbacks(this Context context, Intent intent,
            Action<Activity, Bundle> created = null,
            Action<Activity> started = null,
            Action<Activity> resumed = null,
            Action<Activity> paused = null,
            Action<Activity> stopped = null,
            Action<Activity> destroyed = null,
            Action<Activity, Bundle> saveInstanceState = null
        )
        {
            StartActivityWithCallbacks(context, intent, null, 
                created, started, resumed, paused, stopped, destroyed, saveInstanceState);
        }

        /// <summary>
        /// Start an activity with callbacks
        /// </summary>
        /// <remarks>
        /// With this method, you can supply callbacks for the started activity's lifecycle.
        /// events.
        /// </remarks>
        /// <param name="context">The context to start the activity from (probably another activity)</param>
        /// <param name="intent">Start activity intent (See <see cref="Context.StartActivity(Intent, Bundle)"/>).</param>
        /// <param name="options">Start activity options (See <see cref="Context.StartActivity(Intent, Bundle)"/>).</param>
        /// <param name="created">Called when the activity is created.</param>
        /// <param name="started">Called when the activity is started .</param>
        /// <param name="resumed">Called when the activity is resumed.</param>
        /// <param name="paused">Called when the activity is paused</param>
        /// <param name="stopped">Called when the activity is stopped.</param>
        /// <param name="destroyed">Called when the activity is destroyed.</param>
        /// <param name="saveInstanceState">Called when the activity saves its state</param>
        public static void StartActivityWithCallbacks(this Context context, Intent intent, Bundle options,
                                                      Action<Activity, Bundle> created = null,
                                                      Action<Activity> started = null,
                                                      Action<Activity> resumed = null,
                                                      Action<Activity> paused = null,
                                                      Action<Activity> stopped = null,
                                                      Action<Activity> destroyed = null,
                                                      Action<Activity, Bundle> saveInstanceState = null
        )
        {
            Callbacks.AddActivityCallbacks(intent, created, started, resumed, paused, stopped, destroyed, saveInstanceState);
            context.StartActivity(intent, options);
        }

        

        static ActivityLifeCycleCallbacks _callbacks;

        static ActivityLifeCycleCallbacks Callbacks
        {
            get
            {
                if (_callbacks == null)
                {
                    var app = Application.Context as Application;
                    if (app == null)
                        throw new InvalidOperationException("Application context is not an Application!");
                    _callbacks = new ActivityLifeCycleCallbacks();
                    app.RegisterActivityLifecycleCallbacks(_callbacks);
                } 
                return _callbacks;
            }
        }

        class ActivityLifeCycleCallbacks : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        {
            const string GUID_KEY = "STARTACTIVITYASYNC_GUID_KEY";

            HashSet<Guid> _saveCallbacks = new HashSet<Guid>();
            Dictionary<Guid, Action<Activity,Bundle>> _createdCallbacks
                = new Dictionary<Guid, Action<Activity,Bundle>>();
            Dictionary<Guid, Action<Activity>> _startedCallbacks
                = new Dictionary<Guid, Action<Activity>>();
            Dictionary<Guid, Action<Activity>> _resumedCallbacks
                = new Dictionary<Guid, Action<Activity>>();
            Dictionary<Guid, Action<Activity>> _pausedCallbacks
                = new Dictionary<Guid, Action<Activity>>();
            Dictionary<Guid, Action<Activity>> _stoppedCallbacks
                = new Dictionary<Guid, Action<Activity>>();
            Dictionary<Guid, Action<Activity>> _destroyedCallbacks
                = new Dictionary<Guid, Action<Activity>>();
            Dictionary<Guid, Action<Activity, Bundle>> _saveInstanceStateCallbacks
                = new Dictionary<Guid, Action<Activity, Bundle>>();


            public void AddActivityCallbacks(Intent intent,
                                             Action<Activity, Bundle> created = null,
                                             Action<Activity> started = null,
                                             Action<Activity> resumed = null,
                                             Action<Activity> paused = null,
                                             Action<Activity> stopped = null,
                                             Action<Activity> destroyed = null,
                                             Action<Activity, Bundle> saveInstanceState = null
            )
            {
                var guid = Guid.NewGuid();
                intent.PutExtra(GUID_KEY, guid.ToByteArray());

                if (created != null)
                    _createdCallbacks[guid] = created;
                if (started != null)
                    _startedCallbacks[guid] = started;
                if (resumed != null)
                    _resumedCallbacks[guid] = resumed;
                if (paused != null)
                    _pausedCallbacks[guid] = paused;
                if (stopped != null)
                    _stoppedCallbacks[guid] = stopped;
                if (destroyed != null)
                    _destroyedCallbacks[guid] = destroyed;
                if (saveInstanceState != null)
                    _saveInstanceStateCallbacks[guid] = saveInstanceState;
            }

            static bool TryGetActivityGuid(Activity activity, out Guid guid)
            {
                var bytes = activity.Intent.GetByteArrayExtra(GUID_KEY);
                if (bytes != null)
                {
                    guid = new Guid(bytes);
                    return true;
                }
                guid = Guid.Empty;
                return false;
            }

            void RestoreSavedInstanceState(Activity activity, Bundle savedInstanceState)
            {
                var bytes = savedInstanceState.GetByteArray(GUID_KEY);
                if (bytes != null)
                {
                    activity.Intent.PutExtra(GUID_KEY, bytes);
                    _saveCallbacks.Remove(new Guid(bytes));
                }
            }

            void SaveInstanceState(Activity activity, Bundle outState)
            {
                var bytes = activity.Intent.GetByteArrayExtra(GUID_KEY);
                if (bytes != null)
                {
                    outState.PutByteArray(GUID_KEY, bytes);
                    _saveCallbacks.Add(new Guid(bytes));
                }
            }

            static T GetCallbackFromDictionary<T>(Activity activity, ref Dictionary<Guid, T> fromDictionary)
            {
                Guid guid;
                if (TryGetActivityGuid(activity, out guid))
                {
                    T result;
                    if (fromDictionary.TryGetValue(guid, out result))
                    {
                        return result;
                    }
                }
                return default(T);
            }



            void CleanActivityCallbacks(Activity activity)
            {
                Guid guid;
                if (TryGetActivityGuid(activity, out guid))
                {
                    if (!_saveCallbacks.Contains(guid)) 
                    {
                        _createdCallbacks.Remove(guid);
                        _startedCallbacks.Remove(guid);
                        _resumedCallbacks.Remove(guid);
                        _pausedCallbacks.Remove(guid);
                        _stoppedCallbacks.Remove(guid);
                        _destroyedCallbacks.Remove(guid);
                        _saveInstanceStateCallbacks.Remove(guid);
                    }
                }
            }

            public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
                if (savedInstanceState != null) 
                    RestoreSavedInstanceState(activity, savedInstanceState);
                
                var callback = GetCallbackFromDictionary(activity, ref _createdCallbacks);
                if (callback != null)
                    callback(activity, savedInstanceState);
            }

            public void OnActivityStarted(Activity activity)
            {
                var callback = GetCallbackFromDictionary(activity, ref _startedCallbacks);
                if (callback != null)
                    callback(activity);
            }

            public void OnActivityResumed(Activity activity)
            {
                var callback = GetCallbackFromDictionary(activity, ref _resumedCallbacks);
                if (callback != null)
                    callback(activity);
            }

            public void OnActivityPaused(Activity activity)
            {
                var callback = GetCallbackFromDictionary(activity, ref _pausedCallbacks);
                if (callback != null)
                    callback(activity);

            }

            public void OnActivityStopped(Activity activity)
            {
                var callback = GetCallbackFromDictionary(activity, ref _stoppedCallbacks);
                if (callback != null)
                    callback(activity);
            }

            public void OnActivityDestroyed(Activity activity)
            {
                var callback = GetCallbackFromDictionary(activity, ref _destroyedCallbacks);
                if (callback != null)
                    callback(activity);
                CleanActivityCallbacks(activity);
            }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            {
                SaveInstanceState(activity, outState);
                var callback = GetCallbackFromDictionary(activity, ref _saveInstanceStateCallbacks);
                if (callback != null)
                    callback(activity, outState);
            }

        }
    }
}

