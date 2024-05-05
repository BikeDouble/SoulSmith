using Godot;
using System;
using FormsMoves;
using FormsStats;

public partial class UnitUIHealthBar : ProgressBar
{
	public void Update(UnitStats stats)
	{
		int curHealth = stats.GetModStat(StatType.CurHealth);
		int maxHealth = stats.GetModStat(StatType.MaxHealth);
		double newHealthValue = (Double)((curHealth * 100)/ maxHealth);
		Value = newHealthValue;
	}
}
