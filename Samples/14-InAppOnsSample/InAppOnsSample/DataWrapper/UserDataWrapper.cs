using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace InAppOnsSample
{
    public class UserDataWrapper
    {
        public User Self { get; private set; }

        public string Id { get; private set; }

        public UserDataWrapper(User user, string id)
        {
            Self = user;
            Id = id;
        }
    }
}
