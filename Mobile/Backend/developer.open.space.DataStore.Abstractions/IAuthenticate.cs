﻿using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions
{
    public interface IAuthenticate
    {
        Task<MobileServiceUser> Authenticate();
    }
}