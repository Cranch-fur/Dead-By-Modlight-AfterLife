using Fiddler;

namespace Dead_By_Modlight_Afterlife
{
    public static class FiddlerCore
    {
        static FiddlerCore()
        {
            FiddlerApplication.BeforeRequest += FiddlerToCatchBeforeRequest;
        }
        private static bool EnsureRootCertificate()
        {
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                    return false;
                if (!CertMaker.trustRootCert())
                    return false;
                FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.cert", null);
                FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.key", null);
            }
            return true;
        }
        public static void Start()
        {
            EnsureRootCertificate();
            CONFIG.IgnoreServerCertErrors = true;
            FiddlerApplication.Startup(new FiddlerCoreStartupSettingsBuilder().ListenOnPort(8888).RegisterAsSystemProxy().ChainToUpstreamGateway().DecryptSSL().OptimizeThreadPool().Build());
        }
        public static void Stop()
        {
            FiddlerApplication.Shutdown();
        }
        public static void FiddlerToCatchBeforeRequest(Session oSession)
        {
            if (oSession.uriContains("api/v1/auth/provider/steam/login?token"))
            {
                oSession.LoadResponseFromFile(Globals.responseFile);
            }
            
            if (oSession.uriContains("api/v1/extensions/eac/generateChallenge"))
            {
                oSession.utilCreateResponseAndBypassServer();
                oSession.oResponse.headers.HTTPResponseCode = 200;
                oSession.utilSetResponseBody("{\"challenge\":\"01000000ecbaecbb6bbb107a6fe986945ab98c75f000ff833605ea37ff66f1d4e30abaffc23e6270d400cdb633a4b756573e9210f029bd07080aa7f155a4b2f6a9b07aa06f6d6c5f59bfbb220adf8a70650038c3af6e7b55f391016bad48482c77a60de5d000f68df2fb0923a422354547c00bc3312edf5e1eb4d6c9e0004400\"}");
            }

            if (oSession.uriContains("api/v1/extensions/eac/validateChallengeResponse"))
            {
                oSession.utilCreateResponseAndBypassServer();
                oSession.oResponse.headers.HTTPResponseCode = 200;
                oSession.utilSetResponseBody("{\"valid\":true,\"stateUpdated\":true}");
            }
        }
    }
}
