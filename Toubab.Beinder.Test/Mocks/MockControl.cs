namespace Toubab.Beinder.Mocks
{
    using System;

    public class MockControl
    {

        public MockControl(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        string _text;

        public string Text
        { 
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                if (TextChanged != null)
                    TextChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler TextChanged;

        int _size;

        public int Size
        { 
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                if (SizeChanged != null)
                    SizeChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler SizeChanged;

        public override string ToString()
        {
            return string.Format("[MockControl: Name={0}]", Name);
        }
    }

}

