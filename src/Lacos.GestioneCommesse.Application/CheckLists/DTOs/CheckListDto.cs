﻿using Lacos.GestioneCommesse.Application.Registry.DTOs;

namespace Lacos.GestioneCommesse.Application.CheckLists.DTOs
{
    public class CheckListDto
    {
        public long? Id { get; set; }
        public string? PictureFileName { get; set; }
        public string? Description { get; set; }
        public long ProductTypeId { get; set; }
        public ProductTypeDto? ProductType { get; set; }
        public long ActivityTypeId { get; set; }
        public ActivityTypeDto? ActivityType { get; set; }
        public ICollection<CheckListItemDto> Items { get; set; }
    }
}
