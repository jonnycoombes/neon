namespace JCS.Neon.Glow.Console
{
    /// <summary>
    ///     An enumeration of DEC line drawing elements used when a console is switched to DEC line drawing mode
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public enum DECLineCodes
    {
        BottomRight = '\x6a',
        TopRight = '\x6b',
        TopLeft = '\x6c',
        BottomLeft = '\x6d',
        CrossBar = '\x6e',
        CenterLine = '\x71',
        LeftBorder = '\x74',
        RightBorder = '\x75',
        BottomBorder = '\x76',
        TopBorder = '\x77',
        VerticalBar = '\x78'
    }
}