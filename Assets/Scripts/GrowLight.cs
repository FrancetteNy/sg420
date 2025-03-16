using System;
[Serializable]
public class Growlight
{
    public GrowlightType Type;
    public bool IsInFloweringGrowthMode;
    public Growlight(GrowlightType type = GrowlightType.NONE, bool isInFloweringGrowthMode = false)
    {
        Type = type;
        IsInFloweringGrowthMode = isInFloweringGrowthMode;
    }
}
[Serializable]
public enum GrowlightType
{
    NONE,
    LED
}
