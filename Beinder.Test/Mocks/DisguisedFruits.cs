using System;

namespace Beinder.Mocks
{
    interface IApple
    {

    }

    interface IPear
    {

    }

    interface IApple<T> : IApple
    {

    }

    interface IBadApple<T> : IApple
    {

    }

    class Pear
    {

    }

    class SpecialPear : Pear
    {
    }

    class Orange
    {

    }

    class DisguisePearAsApple : IApple<Pear>
    {

    };

    class DisguiseSpecialPearAndPearAsApple : IApple<SpecialPear>, IApple<Pear>
    {
    }

    class DisguiseSpecialPearAsApple : IApple<SpecialPear>
    {

    };

    class DisguisePearOrOrangeAsApple : IApple<Pear>, IApple<Orange>
    {

    };

    class DisguisePearInterfaceAsApple : IApple<IPear>
    {

    };

    class DisguisePearAsBadApple : IBadApple<Pear>
    {

    };

    class DisguisePearAndOrangeAsApple : IApple<Pear>, IApple<Orange>
    {

    }
}

