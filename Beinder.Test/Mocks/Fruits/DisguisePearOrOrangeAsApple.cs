using System;

namespace Beinder.Mocks.Fruits
{

    public class DisguisePearOrOrangeAsApple : IApple<Pear>, IApple<Orange>
    {

    };
    
}
