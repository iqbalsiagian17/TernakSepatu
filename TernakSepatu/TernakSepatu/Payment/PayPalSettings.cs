namespace TernakSepatu.Payment
{
    public class PayPalSettings
    {
        public string Mode { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        // Parameterless constructor (required for dynamic instantiation)
        public PayPalSettings()
        {
            // No initialization needed here
        }

        // Constructor with arguments
        public PayPalSettings(string clientId, string clientSecret, string mode)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Mode = mode;
        }

        public string BaseUrl => Mode == "sandbox"
            ? "https://api-m.sandbox.paypal.com"
            : "https://api-m.paypal.com";
    }



}
