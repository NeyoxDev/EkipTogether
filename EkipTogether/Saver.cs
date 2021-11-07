using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace EkipTogether
{
    
    public class Saver
    {
        private static Dictionary<String, String> settings = new Dictionary<String, String>();

        public static void addParam(String key, String value)
        {
            settings[key] = value;
        }

        public static String getValue(String key)
        {
            return settings[key];
        }
        
        public static String getValue(String key, String defaultValue)
        {
            return settings.ContainsKey(key) ? settings[key] : defaultValue;
        }

        public static void saveOptions()
        {
            string json =  JsonConvert.SerializeObject(settings);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"/EKIP/";
            Console.WriteLine("Saving... " + path);

            if (!File.Exists(path) || !File.Exists(path+"options.json"))
            {
                Directory.CreateDirectory(path);
                File.Create(path + "options.json").Close();
            }
            File.WriteAllText(path+"options.json", json);
        }

        public static void loadOptions()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"/EKIP/options.json";
            if (File.Exists(path))
            {
                Dictionary<String, String> data = JsonConvert.DeserializeObject<Dictionary<String, String>>(File.ReadAllText(path, Encoding.UTF8));
                settings = data;
            }
          
            
        }
        
    }


}