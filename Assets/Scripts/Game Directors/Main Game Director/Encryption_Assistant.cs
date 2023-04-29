using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;

/*
 * An assistant file, Encryption Assistant is called by Information_Handler to securely Encrypt and Decrypt Game Data and Game SaveFiles. 
 * The Encyrption assistant uses AES encryption with Zeroes Padding, a secure encryption method that cannot easily be cracked. To encrypt and decrypt the data, the game utilizes a secret key and iv which are present in a
 * configuration file within the Asset folder of the game. Data saved and encrypted is sent to Standard Filepath for most systems.
 */
public static class Encryption_Assistant
{
    // The Secret Key and IV, private static variables so that they cannot be accessed by any other script
    private static byte[] secret_key;
    private static byte[] iv;


    // Encrypts GameData, taking in the data as input, the filepath to save it to and the secret key and iv string read from the game.
    public static void Encrypt_GameData(string input, string filepath, string secret_key_string, string iv_string)
    {
        byte[] encrypted_data;

        secret_key = Encoding.UTF8.GetBytes(secret_key_string);
        iv =         Encoding.UTF8.GetBytes(iv_string);
        
        // C# / Unity has built-in AES support, which comes in very handy when having to use protection or GDPR complaince
        using (var aes_encryption = Aes.Create())
        {
            aes_encryption.Key = secret_key;
            aes_encryption.IV = iv;
            aes_encryption.Padding = PaddingMode.Zeros;

            // Start the Memory Stream, then use an encryptor to create a Crypto Stream, which allows to encrypt the data alongside the Memory Stream, then save the contents of the CryptoStream using a StreamWriter.
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

                // This is just a system message for debugging purposes.
                encrypted_data = memory_stream.ToArray();
                Debug.Log("This is the incripted data: " + Convert.ToBase64String(encrypted_data));

                File.WriteAllBytes(filepath, encrypted_data);
            }
        }

        

        Debug.Log("Data Encrypted and saved Securely!");
    }

    // Gamedata is decrypted following a similar process to encryption. Retrive the encrypted data and then decrypte it with a Decryptor, instead of an Encryptor, in a CryptoStream that subsequently gets read via a StreamReader
    public static string Decrypt_GameData(string filepath, string secret_key_string, string iv_string)
    {
        byte[] encrypted_data_array = File.ReadAllBytes(filepath);

        secret_key = Encoding.UTF8.GetBytes(secret_key_string);
        iv = Encoding.UTF8.GetBytes(iv_string);

        // Don't Worry! The data isn't simply saved in base64. If you try to conver encrypted_data_array using base64, your result will be absolute gibberish. It needs to be decrypted!
        Debug.Log("This is the incripted data: " + Convert.ToBase64String(encrypted_data_array));
        string decrypted_data;

        using (var aes_decryption = Aes.Create())
        {
            aes_decryption.Key = secret_key;
            aes_decryption.IV = iv;
            aes_decryption.Padding = PaddingMode.Zeros;

            var decryptor = aes_decryption.CreateDecryptor(aes_decryption.Key, aes_decryption.IV);

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
