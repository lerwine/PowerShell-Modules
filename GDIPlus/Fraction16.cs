using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Threading;

namespace Erwine.Leonard.T.GDIPlus
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Fraction16 : IEquatable<Fraction16>, IComparable<Fraction16>, IFraction<short>
    {
    }
}
