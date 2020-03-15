namespace ConsoleLoop
{
    public static class InColorExtensions
    {
        public static string InColor(this string t, string color) => string.Concat(color, t, AnsiColors.Reset);

        public static string InRed(this string t) => t.InColor(AnsiColors.Red);
        public static string InCyan(this string t) => t.InColor(AnsiColors.Cyan);
        public static string InBlue(this string t) => t.InColor(AnsiColors.Blue);
        public static string InGreen(this string t) => t.InColor(AnsiColors.Green);
        public static string InMagenta(this string t) => t.InColor(AnsiColors.Magenta);
        public static string InWhite(this string t) => t.InColor(AnsiColors.White);
        public static string InYellow(this string t) => t.InColor(AnsiColors.Yellow);
        public static string InBlack(this string t) => t.InColor(AnsiColors.Black);
    }
}
