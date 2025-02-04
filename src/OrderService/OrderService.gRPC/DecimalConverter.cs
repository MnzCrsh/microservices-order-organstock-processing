namespace OrderService.gRPC;

public static class DecimalConverter
{
    /// <summary>
    /// Combines bytes sent from protobuf into decimal value
    /// </summary>
    /// <param name="lo">Low 32 bytes</param>
    /// <param name="hi">Last 32 bytes</param>
    /// <param name="signScale">Sign</param>
    public static decimal FromProtobuf(ulong lo, uint hi, int signScale)
    {
        uint loLow = (uint)(lo & 0xFFFFFFFF);
        uint loHigh = (uint)(lo >> 32);

        byte scale = (byte)(signScale & 0xFF);
        bool isNegative = (signScale & 0x80000000) != 0;

        return new decimal(
            lo: (int)loLow,
            mid: (int)loHigh,
            hi: (int)hi,
            isNegative: isNegative,
            scale: scale
        );
    }

    /// <summary>
    /// Splits decimal into components for protobuf serialization
    /// </summary>
    /// <param name="value">Decimal value to convert</param>
    public static (ulong lo, uint hi, int signScale) ToProtobuf(decimal value)
    {
        var bits = decimal.GetBits(value);

        int loPart = bits[0];
        int midPart = bits[1];
        int hiPart = bits[2];
        int flags = bits[3];

        bool isNegative = (flags & 0x80000000) != 0;
        byte scale = (byte)((flags >> 16) & 0xFF);

        ulong lo = ((ulong)(uint)midPart << 32) | (uint)loPart;
        uint hi = (uint)hiPart;
        int signScale = (int)((isNegative ? 0x80000000 : 0) | scale);

        return (lo, hi, signScale);
    }
}