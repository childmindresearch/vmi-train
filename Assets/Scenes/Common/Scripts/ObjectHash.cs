using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// Provides methods for computing hashes of objects.
/// </summary>
public static class ObjectHash
{
    /// <summary>
    /// Computes the SHA256 hash of the given object by serializing it to JSON
    /// and hashing the resulting bytes.
    /// </summary>
    /// <param name="obj">The object to compute the hash for.</param>
    /// <returns>The SHA256 hash of the object.</returns>
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
