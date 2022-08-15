﻿using System.Collections.Generic;

namespace BridgeCareCore.Models
{
    public class UserInfo
    {
        public string Name { get; set; }

        public string Role { get; set; }

        public List<string> Claims { get; set; }

        public string InternalRole { get; set; }

        public string Email { get; set; }
    }
}
