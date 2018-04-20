using CloudAudit.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace todo
{
    public static class HelperExtensions
    {
        public static string GetGlyphName(this OperationType operationType)
        {
            switch(operationType)
            {
                case OperationType.Action:
                    return "fa-exclamation-circle";

                case OperationType.Change:
                    return "fa-edit";

                case OperationType.Statement:
                    return "fa-info-circle";

                case OperationType.View:
                    return "fa-eye";

                default:
                    return "fa-tag";
            }
        }
    }
}