﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogSystem
{
    public interface ILogObserver
    {
        void LogUpdate(LogMessage message);
    }
}
