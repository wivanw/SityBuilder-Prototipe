using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HexUnit
{
    public readonly Dictionary<HexUnitObj, object> OtherObjects = new Dictionary<HexUnitObj, object>();
    private readonly ArrayList _hexTriggers = new ArrayList();
    public HexUnit(Biom type)
    {
        Type = type;
    }
    public Biom Type { get; private set; }

    /// <summary>
    /// Launches all metods of co-operation for target if they is
    /// </summary>
    /// <typeparam name="TTarget">Type object for potentially existing triggers</typeparam>
    /// <param name="target">Target of co-operation</param>
    /// <param name="args">Arguments of co-operation if this need</param>
    /// <returns>Is target permissible move to this hex</returns>
    public bool Trigger<TTarget>(TTarget target, EventArgs args = default(EventArgs))
    {
        var query = from HexTriggerContainer<TTarget> trigger in _hexTriggers
                    where trigger.TargetType == typeof(TTarget)
                    select trigger;
        var hexTriggers = query as HexTriggerContainer<TTarget>[] ?? query.ToArray();
        return !hexTriggers.Any() || hexTriggers.All(hexTrigger => hexTrigger.Installer.Trigger(target, args));
    }

    /// <summary>
    /// Checks is trigger in this hex unit
    /// </summary>
    /// <typeparam name="TTarget">Type object for potentially existing triggers</typeparam>
    /// <param name="target">Potential target of co-operation</param>
    /// <returns>Is any trigger in hex unit</returns>
    public bool IsTriggerExist<TTarget>(TTarget target)
    {
        var query = from HexTriggerContainer<TTarget> trigger in _hexTriggers
                    where trigger.TargetType == typeof(TTarget)
                    select trigger;
        return query.Any();
    }

    /// <summary>
    /// Add new trigger in hex unit
    /// </summary>
    /// <typeparam name="TTarget">Type object for target trigger</typeparam>
    /// <param name="installer"></param>
    public void AddTrigger<TTarget>(IHexTrigger<TTarget> installer)
    {
        _hexTriggers.Add(new HexTriggerContainer<TTarget>(installer));
    }

    /// <summary>
    /// Remove trigger in hex unit with this installer
    /// </summary>
    /// <typeparam name="TTarget">Type object for target trigger</typeparam>
    /// <param name="installer">object who set trigger in this hex unit</param>
    public void RemoveTriggers<TTarget>(IHexTrigger<TTarget> installer)
    {
        var query = from HexTriggerContainer<TTarget> trigger in _hexTriggers
                    where trigger.Installer.Equals(installer)
                    select trigger;
        var hexTriggerContainers = query as HexTriggerContainer<TTarget>[] ?? query.ToArray();
        if (!hexTriggerContainers.Any())
            return;
        foreach (var trigger in hexTriggerContainers)
        {
            _hexTriggers.Remove(trigger);
        }
    }

    private struct HexTriggerContainer<TTarget>
    {
        internal Type TargetType { get; private set; }
        internal IHexTrigger<TTarget> Installer { get; private set; }

        internal HexTriggerContainer(IHexTrigger<TTarget> installer) : this()
        {
            TargetType = typeof(TTarget);
            Installer = installer;
        }
    }
}

public interface IHexTrigger<in TTarget>
{
    /// <summary>
    /// Metod of co-operation for object who included in hex
    /// </summary>
    /// <param name="target">Object who included in hex</param>
    /// <param name="args">Arguments of co-operation if this need</param>
    ///  <returns>Is target permissible move to this hex</returns>
    bool Trigger(TTarget target, EventArgs args = default(EventArgs));
}

public interface IMovables
{
    int Speed { get; }
    Hex Position { get; }
    void SetPosition(Hex position);
}

public enum Biom { Plain, Forest, Road, Building, Occupied }
public enum HexUnitObj { RoadOrientation }