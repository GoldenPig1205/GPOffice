﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Exiled.API.Interfaces;

namespace GPOffice
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
    }
}