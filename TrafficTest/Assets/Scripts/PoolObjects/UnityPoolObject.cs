using UnityEngine;

public class UnityPoolObject : MonoBehaviour, IPoolObject<string>
{
    public virtual string Group { get { return name; } } // та самая группа
    private Transform _myTransform;

    public Transform MyTransform
    {
        get
        {
            if (_myTransform) return _myTransform;
            _myTransform = transform;
            _myTransform.SetParent(UnityPoolManager.Instance.transform);
            return _myTransform;
        }
    }

    public virtual void SetTransform(Vector3 position, Quaternion rotation)
    {
        MyTransform.position = position;
        MyTransform.rotation = rotation;
    }

    public virtual void Create() // конструктор для пула
    {
        gameObject.SetActive(true);
    }

    public virtual void OnPush() // деструктор для пула
    {
        gameObject.SetActive(false);
    }

    public virtual void Push() // вызов деструктора
    {
        UnityPoolManager.Instance.Push(Group, this);
    }

    public void FailedPush() // не возможно попасть в пул
    {
        Debug.Log("FailedPush"); // !!!
        Destroy(gameObject);
    }
}
