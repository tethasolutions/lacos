﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Operators.DTOs
{
    public class OperatorDocumentReadModel
    {
        public long? Id { get; set; }
        public string? Description { get; set; }
        public string? FileName { get; set; }
    }
}