using FormsStats;
using Godot;
using System;

public partial class Stat
{
    public Stat(int baseValue = 0, StatType statType = StatType.None)
    {
        Type = statType;
        BaseValue = baseValue;
    }

    public StatType Type { get; }
    public int BaseValue;
}
