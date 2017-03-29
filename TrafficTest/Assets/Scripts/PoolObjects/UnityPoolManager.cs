using UnityEngine;

public class UnityPoolManager : MonoBehaviour
{
    public static UnityPoolManager Instance { get; protected set; }

    public int MaxInstanceCount = 128;

    protected PoolManager<string, UnityPoolObject> PoolManager;

    protected virtual void Awake()
    {
        Instance = this;
        PoolManager = new PoolManager<string, UnityPoolObject>(MaxInstanceCount);
    }


    public virtual bool CanPush()
    {
        return PoolManager.CanPush();
    }

    public virtual bool Push(string groupKey, UnityPoolObject poolObject)
    {
        return PoolManager.Push(groupKey, poolObject);
    }

    public virtual T PopOrCreate<T>(T prefab) where T : UnityPoolObject
    {
        return PopOrCreate(prefab, Vector3.zero, Quaternion.identity);
    }

    public virtual T PopOrCreate<T>(T prefab, Vector3 position, Quaternion rotation) where T : UnityPoolObject
    {
        var result = PoolManager.Pop<T>(prefab.Group);
        if (result == null)
        {
            result = CreateObject(prefab, position, rotation);
        }
        else
        {
            result.SetTransform(position, rotation);
        }
        return result;
    }

    public virtual UnityPoolObject Pop(string groupKey)
    {
        return PoolManager.Pop<UnityPoolObject>(groupKey);
    }

    public virtual T Pop<T>() where T : UnityPoolObject
    {
        return PoolManager.Pop<T>();
    }

    public virtual T Pop<T>(PoolManager<string, UnityPoolObject>.Compare<T> comparer) where T : UnityPoolObject
    {
        return PoolManager.Pop(comparer);
    }

    public virtual T Pop<T>(string groupKey) where T : UnityPoolObject
    {
        return PoolManager.Pop<T>(groupKey);
    }

    public virtual bool Contains(string groupKey)
    {
        return PoolManager.Contains(groupKey);
    }

    public virtual void Clear()
    {
        PoolManager.Clear();
    }

    protected virtual T CreateObject<T>(T prefab, Vector3 position, Quaternion rotation) where T : UnityPoolObject
    {
        var go = Instantiate(prefab.gameObject, position, rotation) as GameObject;
        var result = go.GetComponent<T>();
        result.name = prefab.name;
        return result;
    }
}