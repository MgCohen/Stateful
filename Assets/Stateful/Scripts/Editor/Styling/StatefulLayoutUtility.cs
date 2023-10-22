public static class StatefulLayoutUtility
{
    public static string GetLedIconName(LedStyle led)
    {
        switch (led)
        {
            case LedStyle.green:
                return "d_greenLight";
            case LedStyle.red:
                return "d_redLight";
            case LedStyle.off:
            default:
                return "d_lightOff";
        }
    }
}