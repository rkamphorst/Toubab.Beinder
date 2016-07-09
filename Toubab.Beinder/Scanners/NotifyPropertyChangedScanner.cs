namespace Toubab.Beinder.Scanners
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Bindables;
    using Paths;

    public class NotifyPropertyChangedScanner : TypeScanner
    {
     
        IFragmentParser _syllableParser = new CamelCaseSyllableParser();

        public IFragmentParser PathParser
        { 
            get { return _syllableParser; }
            set { _syllableParser = value; }
        }

        public override IEnumerable<IBindable> Scan(Type type)
        {
            var isNotifyPropertyChanged =
                typeof(INotifyPropertyChanged).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

            if (!isNotifyPropertyChanged)
                return Enumerable.Empty<IProperty>();

            var evt =
                type.GetRuntimeEvent("PropertyChanged");
            
            return
                type.GetRuntimeProperties()
                    .Where(info => 
                        !info.IsSpecialName &&
                        info.GetIndexParameters().Length == 0 &&
                        ((info.GetMethod != null && info.GetMethod.IsPublic) ||
                        (info.SetMethod != null && info.SetMethod.IsPublic))
            )
                    .Select(prop => new ReflectedProperty(
                        _syllableParser.Parse(prop.Name), 
                        prop, 
                        evt, 
                        CreateBroadcastFilter(prop.Name))
                    );
        }

        Func<object[], bool> CreateBroadcastFilter(string propertyName)
        {
            return (payload) =>
            {
                var pcArgs = payload.Length > 1 ? payload[1] as PropertyChangedEventArgs : null;
                return pcArgs != null && (
                    string.IsNullOrEmpty(pcArgs.PropertyName) || Equals(pcArgs.PropertyName, propertyName)
                );
            };
        }

    }
    
}
