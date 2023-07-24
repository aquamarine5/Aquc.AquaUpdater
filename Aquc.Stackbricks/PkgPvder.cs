﻿using Aquc.Stackbricks.MsgPvder;
using Aquc.Stackbricks.PkgPvder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquc.Stackbricks;

public interface IStackbricksPkgPvder
{
    public string PkgPvderId { get; }
    public Task<StacebricksUpdatePackage> DownloadPackageAsync(StackbricksUpdateMessage updateMessage,string savePosition);
}

public class StackbricksPkgPvderManager
{
    static Dictionary<string, IStackbricksPkgPvder> matchDict = new Dictionary<string, IStackbricksPkgPvder>
    {
        {"stbks.pkgpvder.ghproxy",new GhProxyPkgPvder() }
    };
    public static IStackbricksPkgPvder ParsePkgPvder(string pkgPvderId)
    {
        // ncpe
        return matchDict[pkgPvderId];
    }
}