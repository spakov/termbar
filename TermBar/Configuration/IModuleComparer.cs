using System.Collections.Generic;
using TermBar.Configuration.Json;

namespace TermBar.Configuration {
  internal class IModuleComparer : IComparer<IModule> {
    public int Compare(IModule? a, IModule? b) => a!.Order.CompareTo(b!.Order);
  }
}
