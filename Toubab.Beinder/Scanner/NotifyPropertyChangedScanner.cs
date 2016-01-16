using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using Toubab.Beinder.PathParser;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.Scanner
{

    public class NotifyPropertyChangedScanner : TypeScanner
    {
     
        IPathParser _pathParser = new CamelCasePathParser();

        public IPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public override IEnumerable<IBindable> Scan(Type type)
        {
            var isNotifyPropertyChanged =
                typeof(INotifyPropertyChanged).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

            if (!isNotifyPropertyChanged)
                return Enumerable.Empty<IBindableState>();

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
                    .Select(prop => new ReflectedProperty(_pathParser, prop, evt, CreateBroadcastFilter(prop.Name)));
        }

        Func<BindableBroadcastEventArgs, bool> CreateBroadcastFilter(string propertyName)
        {
            return (bcArgs) =>
            {
                var pcArgs = bcArgs.Payload.Length > 1 ? bcArgs.Payload[1] as PropertyChangedEventArgs : null;
                return pcArgs != null && (
                    string.IsNullOrEmpty(pcArgs.PropertyName) || Equals(pcArgs.PropertyName, propertyName)
                );
            };
        }

    }
    
}
