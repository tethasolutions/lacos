﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lacos.GestioneCommesse.Application.Products.DTOs
{
    public class ProductDocumentReadModel
    {
        public long? Id { get; set; }
        public string? OriginalFilename { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
    }
}
