
using System.Reflection.Metadata;

[assembly: MetadataUpdateHandler(typeof(HotReloader))]
public static class HotReloader
{
  static void UpdateApplication()
  {

  }

  // public static event Action? OnReload;
  private static Dictionary<Type, List<Action>> OnReload = [];
  public static void Register(Type type, Action action)
  {
    if (!OnReload.ContainsKey(type))
    {
     OnReload[type] = []; 
    }

    OnReload[type].Add(action);
  }

  public static void ClearCache(Type[]? updatedTypes)
  {

  }

  public static void UpdateApplication(Type[]? updatedTypes)
  {
    // called after — re-render UI, re-resolve services, etc.
    foreach (var t in updatedTypes ?? [])
    {
      OnReload.TryGetValue(t, out var actions);
      if (actions != null)
      {
        Console.WriteLine($"updatedType = {t} {actions.Count()}");
        foreach (var action in actions)
        {
          action();
        }
      }
    }
  }

}
