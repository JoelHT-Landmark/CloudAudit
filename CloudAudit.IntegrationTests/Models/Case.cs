// <copyright file="Case.cs" company="Landmark Information Group Ltd">
// Copyright (c) 2018 All Rights Reserved
// </copyright>
// <author>Joel Hammond-Turner</author>
// <summary>Sample domain class representing a Case</summary>
using System;

namespace CloudAudit.IntegrationTests.Models
{
    class Case
    {
        public Case()
        {
            this.SysRef = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(8);
        }

        public string SysRef { get; internal set; }
    }
}
