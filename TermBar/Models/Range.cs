namespace TermBar.Models {
  /// <summary>
  /// A range of integers.
  /// </summary>
  internal struct Range(int low, int high) {
    public int Low = low;
    public int High = high;
  }
}
