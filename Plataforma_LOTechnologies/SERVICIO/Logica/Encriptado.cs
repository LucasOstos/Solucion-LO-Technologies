using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SERVICIO.Logica
{
    public class Encriptado
    {
        private static Encriptado instancia;
        public static Encriptado Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new Encriptado();
                }
                return instancia;
            }
        }
        private static readonly byte[] clave = Encoding.UTF8.GetBytes("claveAES_1234567");
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("vectorInit654321");
        public string EncriptarContraseña(string contraseña) //SHA256
        {
            using (SHA256 encrypt = SHA256.Create())
            {
                byte[] bytes = encrypt.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                StringBuilder SB = new StringBuilder();
                for (int x = 0; x < bytes.Length; x++)
                {
                    SB.Append(bytes[x].ToString("x2"));
                }
                return SB.ToString();
            }
        }
        public string EncriptacionReversible(string pTexto) //AES
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = clave;
                aes.IV = iv;
                ICryptoTransform encriptador = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encriptador, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(pTexto);
                            sw.Close();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
        }
        public string DesencriptacionReversible(string pTexto) //AES
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = clave;
                aes.IV = iv;
                ICryptoTransform desencriptador = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(pTexto)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, desencriptador, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
