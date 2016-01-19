using System;
using Toubab.Beinder.Tools;

namespace Toubab.Beinder.Mock
{
    public class MockView
    {
        MockControl _control;

        public MockControl Control
        { 
            get
            { 
                return _control; 
            }
            set
            {
                _control = value; 
                if (ControlChanged != null)
                    ControlChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler ControlChanged;

        int _specialProperty;

        public void SetSpecialProperty(int value)
        {
            _specialProperty = value;
        }

        public int GetSpecialProperty()
        {
            return _specialProperty;
        }

        string _specialProperty2;

        public void SetSpecialProperty2(string value)
        {
            _specialProperty2 = value;
        }

        public string GetSpecialProperty2()
        {
            return _specialProperty2;
        }

        public void OnClick()
        {
            var evt = Click;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        public event EventHandler Click;

        public void OnBogus()
        {
            var evt = Bogus;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        public event EventHandler Bogus;


        CommandSource _bogusCommand;

        public CommandSource BogusCommand
        {
            get
            {
                if (_bogusCommand == null)
                {
                    _bogusCommand = new CommandSource(enabled => BogusEnabled = enabled);
                    Bogus += (s, e) => _bogusCommand.OnExecute(null);
                }
                return _bogusCommand;
            }
        }

        public bool BogusEnabled { get; private set; }

    }

}

