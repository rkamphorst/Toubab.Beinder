using System;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Toubab.Beinder.Util
{
    public static class TypeUtilities
    {
        public static Regex BaseClassNameRegex = new Regex(@"^[^\s\.\+`]+");

        /// <summary>
        /// Gets the "generic adaptee argument" from a for a given <paramref name="adapterType"/>
        /// </summary>
        /// <remarks>
        /// Say, you have an interface for an adapter, <typeparamref name="IAdapter"/>
        /// This interface generally adapts a certain type to that interface.
        /// 
        /// Which type is this "certain type"? A nice and decoupled way of indicating
        /// this is have another interface with a generic argument, IAdapter&lt;T&gt;,
        /// T being the adaptee type. 
        /// 
        /// This method looks for an immediate descendant interface of 
        /// <typeparamref name="IAdapter"/> that has exactly one generic argument. 
        /// That generic argument is then assumed to be the "adaptee argument",
        ///  and is returned to the caller.
        /// 
        /// If this method finds no qualifying types, it returns null.
        /// 
        /// ### Example ###
        /// 
        /// You have two interfaces:
        /// 
        ///     public interface IApple { /* interface members */ }
        ///     public interface IApple&lt;T&gt; { /* probably empty */ }
        /// 
        /// When you implement the class as follows:
        /// 
        ///     public class Pear { /* pear members */ }
        ///     public class DisguisePearAsApple : IApple&lt;Pear&gt; { 
        ///         /* implementation of IApple members */
        ///     };
        /// 
        /// And run the following:
        /// 
        ///     Type adapteeType =
        ///        typeof(DisguisePearAsApple).GetGenericAdapteeArgument<IApple>();
        /// 
        /// Then <c>adapteeType</c> will contain a <cref name="Type"/> object
        /// representing <c>Pear</c>.
        /// 
        /// Note that it is allowed (and probable) that the adaptee type is 
        /// an interface type; it doesn't have to be a concrete type like
        /// <c>Pear</c>.
        /// 
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if given 
        /// <typeparamref name="IAdapter"/> is not an interface type.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the 
        /// <paramref name="adapterType"/> implements multiple
        /// interfaces with one generic argument that are immediate descendants 
        /// of <typeparamref name="IAdapter"/>.  </exception>
        /// <returns>The generic adaptee argument.</returns>
        /// <param name="adapterType">Adapter type.</param>
        /// <typeparam name="IAdapter">The 1st type parameter.</typeparam>
        public static IEnumerable<Type> EnumerateGenericAdapteeArguments<IAdapter>(this Type adapterType)
            where IAdapter : class
        {
            var tadapter = typeof(IAdapter).GetTypeInfo();
            var tadapterBaseName = BaseClassNameRegex.Match(tadapter.Name).Value;

            if (!tadapter.IsInterface)
                throw new ArgumentException("The TAdapter generic type argument needs to be an interface");

            TypeInfo info = adapterType.GetTypeInfo();

            return
                    info.ImplementedInterfaces
                        .Select(i => i.GetTypeInfo())
                        .Where(i => 
                            tadapter.IsAssignableFrom(i) &&
                            Equals(BaseClassNameRegex.Match(i.Name).Value, tadapterBaseName) &&
                            i.GenericTypeArguments.Length == 1
                        )
                        .Select(i => i.GenericTypeArguments[0]);
            
        }

    }
}

