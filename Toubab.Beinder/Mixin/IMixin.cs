namespace Toubab.Beinder.Mixin
{
    using Annex;

    /// <summary>
    /// Interface for a mix-in
    /// </summary>
    /// <remarks>
    /// A mix-in is a class that defines methods, events and properties
    /// that are "mixed in" with another class that it is a mix-in for.
    /// 
    /// In other words, a mix-in is a way of separately specifying extensions for
    /// another class.
    /// 
    /// In the context of Beinder, the <see cref="Scanner.MixinScanner"/> class
    /// can scan for mix-ins (actually, it scans vor <see cref="Mixin{T}"/> implementations).
    /// The mix-ins are themselves scanned as if they were normal objects; the 
    /// <see cref="Scanner.MixinScanner"/> then transforms the resulting <see cref="Bindable.IBindable"/>
    /// objects into bindables that "pretend" to live on the type the mix-ins are
    /// for.
    /// </remarks>
    public interface IMixin : IAnnex
    {

    }

    /// <summary>
    /// Interface for a mix-in for type <typeparamref cref="T"/>
    /// </summary>
    /// <remarks>
    /// The type parameter <typeparamref cref="T" /> serves as a marker that is
    /// used by <see cref="Tools.TypeAdapterFactory{IMixin}"/> and <see cref="Tools.TypeAdapterRegistry{IMixin}"/>
    /// to automatically find the right mix-in for a given type.
    /// </remarks>
    /// <typeparam name="T">Type this is a mix-in for</typeparam>
    public interface IMixin<T> : IMixin 
    {
    }

}

