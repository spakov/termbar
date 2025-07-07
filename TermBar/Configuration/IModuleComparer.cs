using System.Collections.Generic;
using TermBar.Configuration.Json;

namespace TermBar.Configuration {
  /// <summary>
  /// Allows comparing <see cref="IModule"/>s by their <see
  /// cref="IModule.Order"/>.
  /// </summary>
  internal class IModuleComparer : IComparer<IModule> {
    public int Compare(IModule? a, IModule? b) => a!.Order.CompareTo(b!.Order);
  }
}
