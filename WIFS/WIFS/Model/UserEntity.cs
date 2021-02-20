using System;
using System.Collections.Generic;

namespace WIFS
{
    public class Users
    {
        public IList<UserEntity> userList { get; set; }
    }
    public class UserEntity
    {
        public string id { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string _id { get; set; }
        public string depart { get; set; }
        public string departNo { get; set; }
        public string result { get; set; }
    }
}