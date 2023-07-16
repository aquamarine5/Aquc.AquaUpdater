﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquc.Stackbricks;


public class StackbricksActionData
{
    public string Id;
    public List<string> Args;
    public List<string> Flags;
    public StackbricksActionData(string id, List<string> args, List<string> flags)
    {
        Id = id;
        Args = args;
        Flags = flags;
    }
    public StackbricksActionData(string id)
    {
        Id = id;
        Args = new List<string>();
        Flags = new List<string>();
    }
    public bool ContainFlag(string i) => Flags.Contains(i);
}
public class StackbricksActionManager
{
    static readonly Dictionary<string, IStackbricksAction> DefaultActions=new()
    {
        { "stbks.action.open", new ActionOpen()},
        {"stbks.action.replaceall",new ActionReplaceAll() },
        {"stbks.action.runupdpkgactions",new ActionRunUpdatePackageActions() }
    };
    public Dictionary<string, IStackbricksAction> Actions;
    public StackbricksActionManager() 
    {
        Actions = DefaultActions;
    }
    public StackbricksActionManager(Dictionary<string, IStackbricksAction> actions)
    {
        Actions = DefaultActions.Concat(actions).ToDictionary(x => x.Key, x => x.Value);
    }
    public static IStackbricksAction ParseStatic(string id)
    {
        if (DefaultActions.TryGetValue(id, out IStackbricksAction? value))return value;
        else throw new ArgumentException();
    }
    public IStackbricksAction Parse(string id, StackbricksActionData stackbricksAction)
    {
        if(Actions.TryGetValue(id,out IStackbricksAction? value))
            return value;
        else
            throw new ArgumentException();
    }
    
}
public class StackbricksActionList
{
    public class StackbricksActionListConfig
    {
        public List<StackbricksActionData> actions=new();
    }
    public List<StackbricksActionData> actions;
    public StackbricksActionList(string PkgConfigFile)
    {
        using var fs = new FileStream(PkgConfigFile, FileMode.Open, FileAccess.Read);
        using var sr=new StreamReader(fs);
        actions=JsonConvert.DeserializeObject<StackbricksActionListConfig>(sr.ReadToEnd()).actions;
    }
    public StackbricksActionList()
    {
        actions = new List<StackbricksActionData>
        {
            new StackbricksActionData("stbks.action.replaceall")
        };
    }
    public void ExecuteList(StacebricksUpdatePackage updatePackage)
    {
        foreach (var actionData in actions)
        {
            var action = StackbricksActionManager.ParseStatic(actionData.Id);
            action.Execute(actionData,updatePackage);
        }
    }
}
public interface IStackbricksAction
{
    public string Id { get; }
    public void Execute(StackbricksActionData stackbricksAction, StacebricksUpdatePackage updatePackage);
}
public class ActionOpen : IStackbricksAction
{
    public string Id => "stbks.action.open";
    public void Execute(StackbricksActionData stackbricksAction, StacebricksUpdatePackage updatePackage)
    {

        throw new NotImplementedException();
    }
}
public class ActionRunUpdatePackageActions : IStackbricksAction
{

    public string Id => "stbks.action.runupdpkgactions";
    public void Execute(StackbricksActionData stackbricksAction, StacebricksUpdatePackage updatePackage)
    {
        throw new NotImplementedException();
    }
}
public class ActionReplaceAll : IStackbricksAction
{
    public string Id => "stbks.action.replaceall";
    public void Execute(StackbricksActionData stackbricksAction, StacebricksUpdatePackage updatePackage)
    {
        CopyDirectory(updatePackage.depressedDir,updatePackage.programDir);
        if (!stackbricksAction.ContainFlag("stbks.action.replaceall.keepzipfile"))
            File.Delete(updatePackage.zipFile);
    }
    private void CopyDirectory(DirectoryInfo directory, DirectoryInfo dest)
    {
        if (!dest.Exists) dest.Create();
        foreach (FileInfo f in directory.GetFiles())
        {
            f.CopyTo(Path.Combine(dest.FullName, f.Name), true);
        }
        foreach (DirectoryInfo d in directory.GetDirectories())
        {
            CopyDirectory(d, new DirectoryInfo(Path.Combine(dest.FullName, d.Name)));
        }
    }
}
public abstract class StackbricksBaseAction
{
    public abstract string Id { get; }

    public List<string> Args { get;}
    public List<string> Flags { get; }
    public StackbricksBaseAction(StackbricksActionData stackbricksAction)
    {
        Args=stackbricksAction.Args;
        Flags=stackbricksAction.Flags;
    }
}