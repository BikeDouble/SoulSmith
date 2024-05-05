using Godot;
using System;

public partial class Wrapper<T> : GodotObject
{
    public Wrapper(T item)
    {
        _item = item;
    }

    private T _item;

    public T Value { get { return _item; } }

    public static implicit operator T(Wrapper<T> wrapper) {  return wrapper.Value; }
}
