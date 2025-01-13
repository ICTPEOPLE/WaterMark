using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DocMng_Api
{
    public enum UploadAction
    {
        /// <summary>
        /// Count summary
        /// </summary>
        [Description("Count Description")]
        File,
        View,
        Thumbnail
    }
    /// <summary>
    /// PageAction Summary
    /// </summary>
    public  enum PageAction
    {
        /// <summary>
        /// Count summary
        /// </summary>
        [Description("Count Description")]
        Count,
        Data,
        FileCheck
    }
    public enum ObjectCreateAction
    {
        Own,
        Links,
        Desktop,
        Children,
        Parents,
        LinkOneLevel
    }

    public enum RetrieveAction
    {
        Own,
        Links,
        Desktop,
        Children,
        Parents,
        AllChildrenAndLinks,
        AllTaskWithManagersFromProject,
        ProcessHistory,
        Revision,
        ObjectAndReference
    }

    public enum PMSAction
    {
        CFT,
        TaskForUsers
    }
    public enum PMSPageAction
    {
        WorkYear,
        MyWorkYear,
        UserSchedule,
        PartDevelop,
        PSOProcess
    }

    public enum LinkType
    {
        GeneralLink,
        DesktopLink,
        LinkToOneLevels,
        LinkToChildren,
        LinkToParent
    }

    public enum LifeCycleAction
    {
        New = 0,
        CheckedIn = 1,
        CheckedOut = 2,
        Release = 3,
        NewRelease = 4,
        NewReleaseWithFile = 5,
        InWork = 8,
        ItemRelease = 9
    }

    public enum ClassAction
    {
        New,
        Lastest
    }

    public enum ResourceType
    {
        Image
    }
    
    public enum DashboardType
    {
        ChartYear,
        ChartYearLookup,
        ObjectsYear,
        TIMDashboardReport,
        TIMDashboardSubReport
    }

    public enum WorkFlowType
    {
        Connectors,
        Nodes,
        Excutors,
        ExcutorGroup,
        CurrentNode
    }
    public enum SmartBoxType
    {
        In,
        Sent,
        Complete
    }

    public enum ProcessSetType
    {
        TaskStatus, 
        Read,
        NodeUsers
    }

    public enum WorkFlowPUTAction
    {
        TaskStatus, 
        Read,
        NodeUsers
    }
    public enum QIMAction
    {
        CopyQIMItems = 0,
        CopyDFMEAItems = 1
    }

    public enum WebHookEventName
    {
        Ping
    }

    public enum GetAuthAction
    {
        Group,
        UserGroup,
        Role,
        UserRole,
        UserAuth
    } 
    
    public enum AuthToolbarAction
    {
        ProfileCard,
        Link
    }

    public enum WorkerType
    {
        Log,
        Member
    }
}
