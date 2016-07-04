namespace Toubab.Beinder.Mocks
{
    using Bindables;
    using Paths;
    using Mixins;

    public class MockBindable : IBindable
    {

        public MockBindable(Syllables nameSyllables)
        {
            NameSyllables = nameSyllables;
        }

        public BindingOperations Capabilities
        {
            get { return BindingOperations.None; }
        }

        #region IMixin implementation

        public void SetObject(object newObject)
        {
            Object = newObject;
        }

        public IMixin CloneWithoutObject()
        {
            return new MockBindable(NameSyllables);
        }

        #endregion

        #region IBindable implementation

        public System.Type[] ValueTypes
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public Syllables NameSyllables
        {
            get;
            private set;
        }

        public object Object
        {
            get;
            private set;
        }

        #endregion
    }
}
