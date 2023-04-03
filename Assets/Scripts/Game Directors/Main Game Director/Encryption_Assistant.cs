using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;

public static class Encryption_Assistant
{

    private static byte[] secret_key;
    private static byte[] iv;


    public static void Encrypt_GameData(string input, string filepath, string secret_key_string, string iv_string)
    {
        byte[] encrypted_data;

        secret_key = Encoding.UTF8.GetBytes(secret_key_string);
        iv =         Encoding.UTF8.GetBytes(iv_string);

        using (var aes_encryption = Aes.Create())
        {
            aes_encryption.Key = secret_key;
            aes_encryption.IV = iv;
            aes_encryption.Padding = PaddingMode.Zeros;

            var encryptor = aes_encryption.CreateEncryptor(aes_encryption.Key, aes_encryption.IV);

            using (var memory_stream = new MemoryStream())
            {


                using (var crypto_stream = new CryptoStream(memory_stream, encryptor, CryptoStreamMode.Write))
                {

                    using (var crypto_save_writer = new StreamWriter(crypto_stream))
                    {
                        crypto_save_writer.Write(input);
                    }

                }

                encrypted_data = memory_stream.ToArray();
                Debug.Log("This is the incripted data: " + Convert.ToBase64String(encrypted_data));

                File.WriteAllBytes(filepath, encrypted_data);
            }
        }

        

        Debug.Log("Data Encrypted and saved Securely!");
    }

    public static string Decrypt_GameData(string filepath, string secret_key_string, string iv_string)
    {
        byte[] encrypted_data_array = File.ReadAllBytes(filepath);

        secret_key = Encoding.UTF8.GetBytes(secret_key_string);
        iv = Encoding.UTF8.GetBytes(iv_string);

        Debug.Log("This is the incripted data: " + Convert.ToBase64String(encrypted_data_array));
        string decrypted_data;

        using (var aes_decryption = Aes.Create())
        {
            aes_decryption.Key = secret_key;
            aes_decryption.IV = iv;
            aes_decryption.Padding = PaddingMode.Zeros;

            var decryptor = aes_decryption.CreateDecryptor(aes_decryption.Key, aes_decryption.IV);
            //byte[] decrypted_data_array;

            using (var memory_stream = new MemoryStream(encrypted_data_array))
            {
                using (var crypto_stream = new CryptoStream(memory_stream, decryptor, CryptoStreamMode.Read))
                {
                    using (var reader = new StreamReader(crypto_stream))
                    {
                        decrypted_data = reader.ReadToEnd();
                    }
                }
            }

        }

        Debug.Log("Save File Loaded!");

        return decrypted_data;
    }
}
