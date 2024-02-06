using System;
using System.Collections.Generic;
using System.Linq;

public static class RNGManager
{
    private static Random _rng;

    private static Random RNG
    {
        get
        {
            if (_rng == null)
                SetSeed(Math.Abs(Guid.NewGuid().GetHashCode()));
            return _rng;
        }
    }
        
    public static int Seed {  get; private set; }
    public static void SetSeed(int seed) => _rng = new Random(Seed = seed);

    public static float Value => (float)RNG.NextDouble();

    public static T From<T>(ICollection<T> list) => list == null || list.Count == 0 ? default : list.ElementAt(RNG.Next(list.Count));
}
