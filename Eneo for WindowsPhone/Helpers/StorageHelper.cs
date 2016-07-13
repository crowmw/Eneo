using Eneo.WindowsPhone.DataModel;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Eneo.WindowsPhone.Helpers
{
    public class StorageHelper
    {
        private static readonly string _fileName = "token";
        private static readonly string _encryptionKey = "Mygd3ngdfcRYpfht10nK3y";

        public static async Task SaveTokenToLocalStorage(TokenResponse tokenResponse)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            //StorageFile file = await localFolder.GetFileAsync(_fileName);
            //if (file == null)
            var file = await localFolder.CreateFileAsync(_fileName);


            string serializedToken = JsonConvert.SerializeObject(tokenResponse);
            await Windows.Storage.FileIO.WriteTextAsync(file, serializedToken);
        }

        public static async Task<TokenResponse> ReadTokenFromLocalStorage()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            try
            {
                file = await localFolder.GetFileAsync(_fileName);
            }
            catch (FileNotFoundException e)
            {
                return null;
            }

            string serializedToken = await Windows.Storage.FileIO.ReadTextAsync(file);
            return JsonConvert.DeserializeObject<TokenResponse>(serializedToken);
        }

        public static async Task DeleteTokenFromLocalStorage()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var file = await localFolder.GetFileAsync(_fileName);
            await file.DeleteAsync();
        }
    }
}