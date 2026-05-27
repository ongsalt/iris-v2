
using System.Reflection.Metadata;

[assembly: MetadataUpdateHandler(typeof(HotReloader))]
public static class HotReloader
{
  static void UpdateApplication()
  {

  }

  public static event Action? OnReload;

  public static void ClearCache(Type[]? updatedTypes)
  {
    // called first — clear stale caches
    OnReload?.Invoke();
  }

  public static void UpdateApplication(Type[]? updatedTypes)
  {
    // called after — re-render UI, re-resolve services, etc.
    foreach (var t in updatedTypes ?? [])
    {
      // react to the specific updated types
    }
  }

}