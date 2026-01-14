/*
    Taken from Velocity API using dnSPY
*/
using System;
using System.Security.Cryptography;
using System.Text;

// Token: 0x02000002 RID: 2
public class AESEncryption
{
    // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
    public static string Encrypt(string plaintext, string password)
    {
        string result;
        try
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                throw new ArgumentException("Plaintext cannot be empty");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty");
            }
            byte[] array = AESEncryption.GenerateRandomBytes(16);
            byte[] array2 = AESEncryption.GenerateRandomBytes(12);
            byte[] key = AESEncryption.DeriveKey(password, array);
            byte[] bytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] array3 = new byte[bytes.Length];
            byte[] array4 = new byte[16];
            using (AesGcm aesGcm = new AesGcm(key))
            {
                aesGcm.Encrypt(array2, bytes, array3, array4, null);
            }
            byte[] array5 = new byte[array.Length + array2.Length + array3.Length + array4.Length];
            Buffer.BlockCopy(array, 0, array5, 0, array.Length);
            Buffer.BlockCopy(array2, 0, array5, array.Length, array2.Length);
            Buffer.BlockCopy(array3, 0, array5, array.Length + array2.Length, array3.Length);
            Buffer.BlockCopy(array4, 0, array5, array.Length + array2.Length + array3.Length, array4.Length);
            result = Convert.ToBase64String(array5);
        }
        catch (Exception)
        {
            throw;
        }
        return result;
    }

    // Token: 0x06000002 RID: 2 RVA: 0x00002168 File Offset: 0x00000368
    public static string Decrypt(string ciphertext, string password)
    {
        string @string;
        try
        {
            if (string.IsNullOrEmpty(ciphertext))
            {
                throw new ArgumentException("Ciphertext cannot be empty");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be empty");
            }
            byte[] array = null;
            try
            {
                array = Convert.FromBase64String(ciphertext);
            }
            catch (FormatException)
            {
                throw;
            }
            int num = 44;
            if (array.Length < num)
            {
                throw new CryptographicException("Invalid ciphertext: data too short");
            }
            byte[] array2 = new byte[16];
            byte[] array3 = new byte[12];
            int num2 = array.Length - 16 - 12 - 16;
            byte[] array4 = new byte[num2];
            byte[] array5 = new byte[16];
            Buffer.BlockCopy(array, 0, array2, 0, 16);
            Buffer.BlockCopy(array, 16, array3, 0, 12);
            Buffer.BlockCopy(array, 28, array4, 0, num2);
            Buffer.BlockCopy(array, 28 + num2, array5, 0, 16);
            byte[] key = AESEncryption.DeriveKey(password, array2);
            byte[] array6 = new byte[num2];
            using (AesGcm aesGcm = new AesGcm(key))
            {
                aesGcm.Decrypt(array3, array4, array5, array6, null);
            }
            @string = Encoding.UTF8.GetString(array6);
        }
        catch (CryptographicException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
        return @string;
    }

    // Token: 0x06000003 RID: 3 RVA: 0x000022A4 File Offset: 0x000004A4
    private static byte[] DeriveKey(string password, byte[] salt)
    {
        byte[] bytes;
        using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
        {
            bytes = rfc2898DeriveBytes.GetBytes(32);
        }
        return bytes;
    }

    // Token: 0x06000004 RID: 4 RVA: 0x000022EC File Offset: 0x000004EC
    private static byte[] GenerateRandomBytes(int length)
    {
        byte[] array = new byte[length];
        using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
        {
            randomNumberGenerator.GetBytes(array);
        }
        return array;
    }

    // Token: 0x04000001 RID: 1
    private const int KeySize = 256;

    // Token: 0x04000002 RID: 2
    private const int SaltSize = 16;

    // Token: 0x04000003 RID: 3
    private const int NonceSize = 12;

    // Token: 0x04000004 RID: 4
    private const int TagSize = 16;

    // Token: 0x04000005 RID: 5
    private const int Iterations = 100000;
}
