using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class ObjectHash
{
    public static string ComputeSha256Hash(object obj)
    {
        string json = JsonUtility.ToJson(obj);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] hashBytes = sha256Hash.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                builder.Append(hashBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}