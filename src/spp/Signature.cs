using System;
using Spp;

namespace Spp {
  internal struct Signature : IEquatable<Signature> {
    private string _key;
    private int _count;

    internal Signature (string key,int count) {
      _key = key;
      _count = count;
    }

    public override int GetHashCode () { return _key.GetHashCode(); }

    public override bool Equals (Object obj)
    {
      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }
      return Equals((Signature) obj);
    }

    public bool Equals(Signature other)
    {
      return other._key.Equals(_key) && other._count.Equals(_count);
    }
  }
}
