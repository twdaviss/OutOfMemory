using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public interface IBuff<T>
{
    T ValueDelta { get; set; }
    void SetBuffValue(T value);
    T GetBuffValue();
}

public class FloatBuff : IBuff<float>
{
    public float ValueDelta { get; set; }

    public FloatBuff(float valueDelta)
    {
        this.ValueDelta = valueDelta;
    }

    public float GetBuffValue()
    {
        return ValueDelta;
    }

    public void SetBuffValue(float value)
    {
        ValueDelta = value;
    }
}

public class IntBuff : IBuff<int>
{
    public int ValueDelta { get; set; }

    public IntBuff(int valueDelta)
    {
        this.ValueDelta = valueDelta;
    }

    public int GetBuffValue()
    {
        return ValueDelta;
    }

    public void SetBuffValue(int value)
    {
        ValueDelta = value;
    }
}
public interface IBuffable<T>
{
    T BaseValue { get; set; }
    List<IBuff<T>> Buffs { get; set; }
    public void AddBuff(IBuff<T> buff);
    public void RemoveBuff(IBuff<T> buff);
    T GetValue();
}
public class BuffableFloat : IBuffable<float>
{
    public float BaseValue { get; set; }
    public List<IBuff<float>> Buffs { get; set; }
    public BuffableFloat(float baseValue)
    {
        BaseValue = baseValue;
        Buffs = new List<IBuff<float>>();
    }
    public float GetValue()
    {
        var value = BaseValue;
        foreach (var buff in Buffs)
        {
            value += buff.GetBuffValue();
        }
        return value;
    }

    public void AddBuff(IBuff<float> buff)
    {
        Buffs.Add(buff);
    }

    public void RemoveBuff(IBuff<float> buff)
    {
        if(Buffs.Remove(buff) != true)
        {
            UnityEngine.Debug.Log("Buff not found");
        }
    }
}

public class BuffableInt : IBuffable<int>
{
    public int BaseValue { get; set; }
    public List<IBuff<int>> Buffs { get; set; }
    public BuffableInt(int baseValue)
    {
        BaseValue = baseValue;
        Buffs = new List<IBuff<int>>();
    }
    public int GetValue()
    {
        var value = BaseValue;
        foreach (var buff in Buffs)
        {
            value += buff.GetBuffValue();
        }
        return value;
    }

    public void AddBuff(IBuff<int> buff)
    {
        Buffs.Add(buff);
    }

    public void RemoveBuff(IBuff<int> buff)
    {
        if (Buffs.Remove(buff) != true)
        {
            UnityEngine.Debug.Log("Buff not found");
        }
    }
}
