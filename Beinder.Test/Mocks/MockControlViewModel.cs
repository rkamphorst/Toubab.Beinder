using System;

namespace Beinder.Mocks
{

    public class MockControlViewModel : NotifyPropertyChanged
    {
        public MockControlViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        string _text;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        int _size;

        public int Size
        {
            get { return _size; }
            set { SetProperty(ref _size, value); }
        }

        public override string ToString()
        {
            return string.Format("[MockControl: Name={0}]", Name);
        }
    }
}
