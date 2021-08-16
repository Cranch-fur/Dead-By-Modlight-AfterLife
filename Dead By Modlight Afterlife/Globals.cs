namespace Dead_By_Modlight_Afterlife
{
    public static class Globals // Class-container of variables, that accessible from any part of code.
    {
        // => SteamAuth response temp file location
        public static string responseFile = System.IO.Path.GetTempPath() + "\\AuthResponse.txt";

        // => DownloadLink to Modding Files, specifed from web request
        public static string moddingFiles = "";

        // => DebugMode
        public static bool isDebugMode = false;
    }
}
