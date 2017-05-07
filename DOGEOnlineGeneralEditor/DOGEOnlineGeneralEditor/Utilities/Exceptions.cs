using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOGEOnlineGeneralEditor.Utilities
{
    public class UserNotFoundException : Exception
    {
    }
    public class ProjectNotFoundException : Exception
    {
    }
    public class FileNotFoundException : Exception
    {
    }
    public class DuplicateProjectNameException : Exception
    {
    }

    public class UnauthorizedAccessToProjectException : Exception
    {
    }
}