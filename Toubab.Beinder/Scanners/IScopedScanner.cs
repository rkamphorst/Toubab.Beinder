namespace Toubab.Beinder.Scanners
{
    public interface IScopedScanner : IScanner
    {
        IScopedScanner NewScope();
    }
}
