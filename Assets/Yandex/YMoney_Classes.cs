using Newtonsoft.Json;
using System;


public partial class YMoney
{
    public class Purchase
    {
        public string ProductID { get; }
        public string PurchaseToken { get; }
        public string DeveloperPayload { get; }

        [JsonConstructor]
        public Purchase(string productID, string purchaseToken, string developerPayload)
        {
            ProductID = productID;
            PurchaseToken = purchaseToken;
            DeveloperPayload = developerPayload;
        }
    }

    private static class DevPayload
    {
        private static readonly CInt prime = 17;
        private static readonly CInt k = 48253;

        private static readonly int bls = 2;
        private static readonly int bln = 3;
        private static readonly int mcn = 3;
        private static readonly int brn = 5;

        public static string Create()
        {
            int ln = (int)Math.Pow(10, bln - 1);
            int rn = (int)Math.Pow(10, brn - 1);
            int cn = (int)Math.Pow(10, mcn - 1);
            URandom.InitState(Helper.Seed);
            return Helper.RandomString(bls) +
                   Convert.ToString(URandom.Range(ln, ln * 10)) +
                   Convert.ToString((URandom.Range(cn, cn*cn) * prime) ^ k) +
                   Convert.ToString(URandom.Range(rn, rn * 10));
        }
        public static bool Check(string devPayload)
        {
            Index l = bls + bln;
            Index r = ^brn;
            int minLength = bls + bln + mcn + brn;

            if (string.IsNullOrEmpty(devPayload))
                return false;
            if (devPayload.Length < minLength)
                return false;
            if(!int.TryParse(devPayload[l..r], out var val))
                return false;
            
            val ^= k;
            int min = (int)Math.Pow(10, mcn - 1) * prime;

            return val >= min && val % prime == 0;
        }
    }
}
