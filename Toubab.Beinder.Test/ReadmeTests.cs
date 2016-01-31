namespace Toubab.Beinder
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using NUnit.Framework;
    using Tools;

    [TestFixture]
    public class ReadmeTests
    {
        IBindings _bindings;

        class ViewClass : NotifyPropertyChanged
        {

            string _myProperty;

            public string MyProperty
            {
                get { return _myProperty; }
                set { SetProperty(ref _myProperty, value); }
            }

            BaseControlClass _myControl;

            public BaseControlClass MyControl
            {
                get { return _myControl; }
                set { SetProperty(ref _myControl, value); }
            }


        }

        class ViewModelClass : NotifyPropertyChanged
        {
            string _myProperty;

            public string MyProperty
            {
                get { return _myProperty; }
                set { SetProperty(ref _myProperty, value); }
            }

            ControlModelClass _myControl;

            public ControlModelClass MyControl
            {
                get { return _myControl; }
                set { SetProperty(ref _myControl, value); }
            }

            string _myControlLabel;

            public string MyControlLabel
            {
                get { return _myControlLabel; }
                set { SetProperty(ref _myControlLabel, value); }
            }
                
            int _entryListCount ;
            public int EntryListCount 
            {
                get { return _entryListCount; }
                set { SetProperty(ref _entryListCount, value); }
            }

            ICommand _slowlyDeleteOneEntryCommand;

            public ICommand SlowlyDeleteOneEntryCommand 
            {
                get 
                {
                    if (_slowlyDeleteOneEntryCommand == null) 
                    {
                        _slowlyDeleteOneEntryCommand = new Command(
                            _ => SlowlyDeleteOneEntry(),
                            _ => EntryListCount > 0
                        );
                    }
                    return _slowlyDeleteOneEntryCommand;
                }
            }

            async Task SlowlyDeleteOneEntry() 
            {
                if (EntryListCount <= 0) return;
                await Task.Delay(2000);
                EntryListCount--;
            }

        }
            
        class BaseControlClass : NotifyPropertyChanged
        {
        }

        class ControlClass : BaseControlClass
        {
            string _label;

            public string Label
            {
                get { return _label; }
                set { SetProperty(ref _label, value); }
            }
        }

        class TextControlClass : BaseControlClass
        {
            string _text;

            public string Text
            {
                get { return _text; }
                set { SetProperty(ref _text, value); }
            }
        }

        class ControlModelClass : NotifyPropertyChanged
        {
            string _label;

            public string Label
            {
                get { return _label; }
                set { SetProperty(ref _label, value); }
            }

            string _text;

            public string Text
            {
                get { return _text; }
                set { SetProperty(ref _text, value); }
            }
        }

        [Test]
        public void Tldr()
        {
            var viewModel = new ViewModelClass();
            var view = new ViewClass();

            /* Create binder */
            var binder = new Binder();

            /* Establish bindings.
            * Store the bindings in this._bindings to make sure it is not garbage collected.
            * This is typically done when the view appears (*not* when it is created!)
            */
            this._bindings = binder.Bind(viewModel, view);

            /* Destroy bindings.
             * Typically when the view disappears.
             */
            this._bindings.Dispose();
            this._bindings = null;
        }

        [Test]
        public void NameBasedBinding()
        {
            var binder = new Binder();

            /* ViewModelClass and ViewClass both have a string property MyProperty,
            * and they both implement INotifyPropertyChanged
            */
            var view = new ViewClass { MyProperty = "aaa" };
            var viewModel = new ViewModelClass { MyProperty = "bbb" };

            this._bindings = binder.Bind(viewModel, view);

            //  viewModel.MyProperty is now bound to view.MyProperty.
            //  view.MyProperty will have the value "bbb".

            Assert.AreEqual("bbb", view.MyProperty);

            view.MyProperty = "ccc";      // propagates "ccc" to viewModel.MyProperty

            Assert.AreEqual("ccc", viewModel.MyProperty);

            viewModel.MyProperty = "ddd"; // propagates "ddd" to view.MyProperty

            Assert.AreEqual("ddd", view.MyProperty);
        }

        [Test]
        public void RecursiveBinding()
        {
            var binder = new Binder();

            /* ViewModelClass, ViewClass, ControlModelClass and ControlClass all 
             * implement interface INotifyPropertyChanged.
             */
            var view = new ViewClass() { MyControl = new ControlClass { Label = "aaa" } };
            var viewModel = new ViewModelClass() { MyControl = new ControlModelClass { Label = "bbb" } };

            this._bindings = binder.Bind(viewModel, view);

            //  viewModel.MyControl.Label is now bound to view.MyControl.Label.
            //  both will have the value "bbb".
            Assert.AreEqual("bbb", ((ControlClass)view.MyControl).Label);

            ((ControlClass)view.MyControl).Label = "ccc";      // propagates "ccc" to viewModel.MyControl.Label

            Assert.AreEqual("ccc", viewModel.MyControl.Label);

            viewModel.MyControl.Label = "ddd"; // propagates "ddd" to view.MyControl.Label

            Assert.AreEqual("ddd", ((ControlClass)view.MyControl).Label);
        }

        [Test]
        public void BindingAcrossObjectBoundaries()
        {
            var binder = new Binder();

            /* ViewModelClass, ViewClass, and ControlClass all implement interface 
             * INotifyPropertyChanged.
             */
            var view = new ViewClass() { MyControl = new ControlClass { Label = "aaa" } };
            var viewModel = new ViewModelClass { MyControlLabel = "bbb" };

            this._bindings = binder.Bind(viewModel, view);

            //  viewModel.MyControlLabel is now bound to view.MyControl.Label.
            //  both will have the value "bbb".
            Assert.AreEqual("bbb", ((ControlClass)view.MyControl).Label);

            ((ControlClass)view.MyControl).Label = "ccc";      // propagates "ccc" to viewModel.MyControlLabel

            Assert.AreEqual("ccc", viewModel.MyControlLabel);

            viewModel.MyControlLabel = "ddd";  // propagates "ddd" to view.MyControl.Label

            Assert.AreEqual("ddd", ((ControlClass)view.MyControl).Label);
        }

        [Test]
        public void DynamicRebinding()
        {
            var binder = new Binder();

            /* ViewModelClass, ViewClass, ControlModelClass, ControlClass and TextControlClass all 
             * implement interface INotifyPropertyChanged.
             */
            var view = new ViewClass() { MyControl = new ControlClass { Label = "aaa" } };
            var viewModel = new ViewModelClass() { MyControl = new ControlModelClass { Label = "bbb", Text = "ccc" } };

            this._bindings = binder.Bind(viewModel, view);

            //  viewModel.MyControl.Label is now bound to view.MyControl.Label.
            //  Both will have the value "bbb".
            //  viewModel.MyControl.Text is not bound to anything and will have its original value ("ccc")

            Assert.AreEqual("bbb", ((ControlClass)view.MyControl).Label);
            Assert.AreEqual("ccc", viewModel.MyControl.Text);

            view.MyControl = new TextControlClass { Text = "ddd" };

            // viewModel.MyControl.Text gets *rebound* to view.MyControl.Text.
            // Both will now have value "ddd".
            // viewModel.MyControl.Label will still have value "bbb".

            Assert.AreEqual("bbb", viewModel.MyControl.Label);
            Assert.AreEqual("ddd", viewModel.MyControl.Text);

        }
    }
}

