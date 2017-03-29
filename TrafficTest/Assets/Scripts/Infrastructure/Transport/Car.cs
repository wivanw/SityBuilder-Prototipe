using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Car : ParentOwner, IHexTrigger<Car>, IMovables
{
    private static Car[] _prefab;
    public static Car[] Prefab
    {
        get { return _prefab ?? (_prefab = Resources.LoadAll<Car>(Constants.PathCar)); }
    }
    public int Speed { get { return Constants.CarSpeed; } }
    private Dictionary<Hex, Biom> _bottomDraft;
    public override Dictionary<Hex, Biom> BottomDraft
    {
        get { return _bottomDraft ?? (_bottomDraft = DraftHelper.Car()); }
        protected set { _bottomDraft = value; }
    }
    public bool IsFilledUp;
    private Coroutine _move;
    private IEnumerable<Hex> _path;
    public Hex TargetFollowing { get { return _path.Last(); } }
    private bool _isAnimation;

    public bool Trigger(Car target, EventArgs args = null)
    {
        return false;
    }

    public void SetPosition(Hex position)
    {
        Position = position;
    }

    public void Move(IEnumerable<Hex> path)
    {
        _path = path;
        if (_move != null)
            StopCoroutine(_move);
        _move = StartCoroutine(StartMove(path.GetEnumerator()));
    }
    private IEnumerator StartMove(IEnumerator<Hex> path)
    {
        var isMove = path.MoveNext();
        while (isMove)
        {
            if (MapManager.Instance.TryMove(this, path.Current))
            {
                yield return StartCoroutine(MoveTransf());
                isMove = path.MoveNext();
            }
            else
                yield return null;
        }
    }

    private IEnumerator MoveTransf()
    {
        while (transform.position != Map.Instance.Layout.HexToVector3(Position))
        {
            transform.forward = Vector3.Lerp(transform.forward, (Map.Instance.Layout.HexToVector3(Position) - transform.position).normalized, Time.deltaTime * Speed * 1.5f);
            transform.position = Vector3.MoveTowards(transform.position, Map.Instance.Layout.HexToVector3(Position), Time.deltaTime * Speed);
            yield return null;
        }
    }

    public void StartAnimation()
    {
        _isAnimation = true;
        var materials = GetComponentInChildren<Renderer>().materials.Where(material => material.shader.name == Constants.SelectableShader);
        StartCoroutine(AnimationCor(materials));
    }

    private IEnumerator AnimationCor(IEnumerable<Material> materials)
    {
        var enumerable = materials as Material[] ?? materials.ToArray();
        while (_isAnimation)
        {
            SetAlpha(enumerable, Mathf.Abs(1 - Time.time % 2));
            yield return null;
        }
        SetAlpha(enumerable, 0);
    }

    private void SetAlpha(IEnumerable<Material> materials, float alpha)
    {
        foreach (var material in materials)
        {
            var color = material.GetColor(Constants.OutlineColor);
            color.a = alpha;
            material.SetColor(Constants.OutlineColor, color);
        }
    }

    public void StopAnimation()
    {
        _isAnimation = false;
    }

    private void OnDestroy()
    {
        Map.Instance.HexUnitMap[Position].RemoveTriggers(this);
    }
}
