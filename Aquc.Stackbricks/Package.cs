﻿using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquc.Stackbricks;

public class StacebricksUpdatePackage
{
    public StackbricksUpdateMessage updateMessage;
    public DirectoryInfo programDir;
    public string zipFile;
    public DirectoryInfo depressedDir;
    public StacebricksUpdatePackage(string zipFile,StackbricksUpdateMessage updateMessage,DirectoryInfo programDir)
    {
        this.zipFile = zipFile;
        this.updateMessage= updateMessage;
        this.programDir= programDir;
        depressedDir=DepressedZipFile();
    }
    protected DirectoryInfo DepressedZipFile()
    {
        var depressedDir = new DirectoryInfo(Path.Combine(programDir.FullName, $".StackbricksUpdatePackage_{updateMessage.version}.depressed"));
        if (!depressedDir.Exists) depressedDir.Create();
        else 
        { 
            depressedDir.Delete(true);
            depressedDir.Create();
        }
        ZipFile.ExtractToDirectory(zipFile, depressedDir.FullName);
        return depressedDir;
    }
    public void ExecuteActions()
    {
        var pkgcfg = depressedDir.GetFiles("Aquc.Stackbricks.pkgcfg.json");
        StackbricksActionList stackbricksActionList;
        if (pkgcfg.Length==0)
        {
            stackbricksActionList = new StackbricksActionList();
        }
        else
        {
            stackbricksActionList = new StackbricksActionList(pkgcfg[0].FullName);
        }
        stackbricksActionList.ExecuteList(this);
    }
}
