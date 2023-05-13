using System;

namespace ConsoleApplication1
{
    [Serializable]
    public class UserDTO
    {
        public string User { get; set; }

        public string Pass { get; set; }

        public UserDTO(string username, string parola)
        {
            User = username;
            Pass = parola;
        }
    }
}