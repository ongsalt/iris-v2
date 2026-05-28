using SignalsDotnet;

public static class SignalExtensions
{
  public static void Trigger<T>(this Signal<T> s)
  {
    s.Value = s.Value;
  }

  public static void Mutate<T>(this Signal<T> s, Action<T> mutate) where T: class
  {
    mutate(s.Value);
    s.Value = s.Value;
  }
}