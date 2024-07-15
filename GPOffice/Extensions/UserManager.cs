using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPOffice
{
    public static class UsersManager
    {
        public static string UsersFileName = "C:/Users/GoldenPig1205/AppData/Roaming/EXILED/Configs/GPOffice/Users.txt";
        public static Dictionary<string, List<string>> UsersCache = new Dictionary<string, List<string>>();

        public static bool AddUser(string UserId, List<string> UserInfo) // gp, exp
        {
            UsersCache[UserId] = UserInfo;

            return true;
        }

        public static void SaveUsers()
        {
            var text = string.Join("\n", UsersCache.Select(x => $"{x.Key};{x.Value[0]};{x.Value[1]}"));

            FileManager.WriteStringToFile(UsersFileName, text);
        }

        public static void LoadUsers()
        {
            var text = FileManager.ReadAllText(UsersFileName);

            if (string.IsNullOrWhiteSpace(text))
                return;

            UsersCache.Clear();

            foreach (var line in text.Split('\n'))
            {
                var parts = line.Split(';');

                if (parts.Length != 3)
                    continue;

                UsersCache.Add(parts[0], new List<string>() { parts[1], parts[2] });
            }
        }
    }
}
