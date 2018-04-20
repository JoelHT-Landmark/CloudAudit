using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CloudAudit.Client.Model;
using todo.Models;

namespace todo.ViewModels
{
    public class ItemDetail : Item
    {
        public ItemDetail(Item item, AuditList auditList)
        {
            this.Id = item.Id;
            this.Name = item.Name;
            this.Description = item.Description;
            this.Completed = item.Completed;
            this.AuditRecords = auditList?.ItemsList ?? new List<AuditRecord>();
        }

        public IEnumerable<AuditRecord> AuditRecords { get; set; }
    }
}