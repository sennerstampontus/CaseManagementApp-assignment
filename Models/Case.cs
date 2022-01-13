﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseManagementApp.Models
{
  enum Status
    {
        Waiting,
        Opened,
        Closed,
    }
    internal class Case
    {
        public string Subject { get; set; } = null!;
        public string Description { get; set; } = null!;
        
        public int CustomerId { get; set; }
        public int AdminId { get; set; }
        public Status State { get; set; }

    }
}