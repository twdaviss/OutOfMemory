using UnityEngine;

[CreateAssetMenu(fileName = "BuffObject", menuName = "ScriptableObjects/BuffObject", order = 1)]
public class BuffObject : ScriptableObject, IBuff<float>
{
    [SerializeField]
    private float valueDelta;

    public float ValueDelta { get => valueDelta; set => valueDelta = value; }
    public BuffObject(float valueDelta)
    {
        ValueDelta = valueDelta;
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