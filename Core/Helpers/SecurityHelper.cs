using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace Core.Helpers
{
    public class SecurityHelper
    {
        public static string GetHash(string clearText)
        {
            clearText = clearText ?? string.Empty;

            using (var rijAlg = Rijndael.Create())
            {
                var salt = System.Text.Encoding.ASCII.GetBytes("ab87935000");
                var key = new Rfc2898DeriveBytes(clearText, salt);
                rijAlg.Key = key.GetBytes(rijAlg.KeySize / 8);
                rijAlg.IV = key.GetBytes(rijAlg.BlockSize / 8);

                var bytes = getBytes(clearText, rijAlg);
                return Convert.ToBase64String(bytes);
            }
        }


        private static byte[] getBytes(string clearText, Rijndael cryptoEngine)
        {
            // Create a decrytor to perform the stream transform.
            var encryptor = cryptoEngine.CreateEncryptor(cryptoEngine.Key, cryptoEngine.IV);
            // Create the streams used for encryption.
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Write all data to the stream.
                        swEncrypt.Write(clearText);
                    }

                    return msEncrypt.ToArray();
                }
            }
        }




        public static bool KeyUsageHasUsage(X509Certificate2 cert, X509KeyUsageFlags flag)
        {
            if (cert.Version < 3) { return true; } // Pre version 3 all usages are valid!
            List<X509KeyUsageExtension> extensions = cert.Extensions.OfType<X509KeyUsageExtension>().ToList();
            if (!extensions.Any())
            {
                return flag != X509KeyUsageFlags.CrlSign && flag != X509KeyUsageFlags.KeyCertSign;
            }
            // X509KeyUsageFlags.DigitalSignature + X509KeyUsageFlags.KeyEncipherment
            return (extensions[0].KeyUsages & flag) > 0;
        }


        /// <summary>
        /// Finds a certificate (from localMachine/My) using the findType and findKey supplied.
        /// </summary>
        public static X509Certificate2 Find(X509FindType findType, string findKey, bool validOnly)
        {
            X509Store userCaStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                // find it:
                userCaStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificatesInStore = userCaStore.Certificates;
                X509Certificate2Collection findResult = certificatesInStore.Find(findType, findKey, validOnly);
                if (findResult.Count == 1)
                {
                    return findResult[0];
                }
                else if (findResult.Count > 1)
                {
                    foreach (var cert in findResult)
                    {
                        if (KeyUsageHasUsage(cert, X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment))
                        {
                            //logger.Debug($"Using one of several certificates available: {cert.Thumbprint} for {findKey}");
                            return cert;
                        }
                    }

                    //logger.Error($"More than one certificate with matching key {findKey} but none has the required KeyUsages");
                    throw new ApplicationException("Too many client certificates matching criteria.");
                    // Måske kunne man checke datoerne på certifikaterne og så finde den nyeste ?
                    // findResult[0].GetExpirationDateString() eller  NotAfter
                }
                else
                {
                    //logger.Error($"There are no certificates that matches key {findKey}");
                    throw new ApplicationException("Unable to locate the correct client certificate.");
                }
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                //logger.Error($"Unable to locate certificate with key {findKey}", ex);
                throw;
            }
            finally
            {
                userCaStore.Close();
            }
        }
        public static X509Certificate2 FindBySubjectName(string findKey, bool validOnly = false)
        {
            return Find(X509FindType.FindBySubjectName, findKey, validOnly);
        }
        public static X509Certificate2 FindByThumbprint(string findKey, bool validOnly = false)
        {
            return Find(X509FindType.FindByThumbprint, findKey, validOnly);
        }

    

    }
}
