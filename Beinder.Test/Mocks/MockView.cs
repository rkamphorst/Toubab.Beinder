﻿using System;

namespace Beinder.Mocks
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
    }

}

