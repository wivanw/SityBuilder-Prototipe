using System.Collections.Generic;
using System;

public class PoolManager<TKey, TValue> where TValue : IPoolObject<TKey>
{
    public int MaxInstances { get; protected set; }
    public virtual int InctanceCount { get { return Objects.Count; } }
    public virtual int CacheCount { get { return Cache.Count; } }

    public delegate bool Compare<in T>(T value) where T : TValue;

    protected Dictionary<TKey, List<TValue>> Objects;
    protected Dictionary<Type, List<TValue>> Cache;

    public PoolManager(int maxInstance)
    {
        MaxInstances = maxInstance;
        Objects = new Dictionary<TKey, List<TValue>>();
        Cache = new Dictionary<Type, List<TValue>>();
    }

    public virtual bool CanPush()
    {
        return InctanceCount + 1 < MaxInstances;
    }

    public virtual bool Push(TKey groupKey, TValue value)
    {
        const bool result = false;
        if (CanPush())
        {
            value.OnPush();
            if (!Objects.ContainsKey(groupKey))
            {
                Objects.Add(groupKey, new List<TValue>());
            }
            Objects[groupKey].Add(value);
            var type = value.GetType();
            if (!Cache.ContainsKey(type))
            {
                Cache.Add(type, new List<TValue>());
            }
            Cache[type].Add(value);
        }
        else
        {
            value.FailedPush();
        }
        return result;
    }

    public virtual T Pop<T>(TKey groupKey) where T : TValue
    {
        var result = default(T);
        if (Contains(groupKey) && Objects[groupKey].Count > 0)
        {
            for (var i = 0; i < Objects[groupKey].Count; i++)
            {
                if (Objects[groupKey][i] is T)
                {
                    result = (T)Objects[groupKey][i];
                    var type = result.GetType();
                    RemoveObject(groupKey, i);
                    RemoveFromCache(result, type);
                    result.Create();
                    break;
                }
            }
        }
        return result;
    }

    public virtual T Pop<T>() where T : TValue
    {
        var result = default(T);
        var type = typeof(T);
        if (ValidateForPop(type))
        {
            for (var i = 0; i < Cache[type].Count; i++)
            {
                result = (T)Cache[type][i];
                if (result != null && Objects.ContainsKey(result.Group))
                {
                    Objects[result.Group].Remove(result);
                    RemoveFromCache(result, type);
                    result.Create();
                    break;
                }

            }
        }
        return result;
    }

    public virtual T Pop<T>(Compare<T> comparer) where T : TValue
    {
        var result = default(T);
        var type = typeof(T);
        if (ValidateForPop(type))
        {
            for (var i = 0; i < Cache[type].Count; i++)
            {
                var value = (T)Cache[type][i];
                if (comparer(value))
                {
                    Objects[value.Group].Remove(value);
                    RemoveFromCache(result, type);
                    result = value;
                    result.Create();
                    break;
                }

            }
        }
        return result;
    }


    public virtual bool Contains(TKey groupKey)
    {
        return Objects.ContainsKey(groupKey);
    }

    public virtual void Clear()
    {
        Objects.Clear();
    }

    protected virtual bool ValidateForPop(Type type)
    {
        return Cache.ContainsKey(type) && Cache[type].Count > 0;
    }

    protected virtual void RemoveObject(TKey groupKey, int idx)
    {
        if (idx >= 0 && idx < Objects[groupKey].Count)
        {
            Objects[groupKey].RemoveAt(idx);
            if (Objects[groupKey].Count == 0)
            {
                Objects.Remove(groupKey);
            }
        }
    }

    protected void RemoveFromCache(TValue value, Type type)
    {
        if (Cache.ContainsKey(type))
        {
            Cache[type].Remove(value);
            if (Cache[type].Count == 0)
            {
                Cache.Remove(type);
            }
        }
    }
}

public interface IPoolObject<out T>
{
    T Group { get; }
    void Create();
    void OnPush();
    void FailedPush();
}