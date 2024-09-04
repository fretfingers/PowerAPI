using DevExpress.XtraRichEdit.Import.OpenXml;
using System.Collections.Generic;

namespace PowerAPI.Data.POCO
{
    public class NavigationDto
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public bool IsSelected { get; set; }
        public bool IsFavourite { get; set; }
        public List<NavigationDto> SubNav { get; set; } = new List<NavigationDto>();
        public MetaData MetaData { get; set; } = new MetaData();
        public int UserOrder { get; set; }
        public int ParentId { get; set; }
    }

    public class MetaData
    {
        public string Icon { get; set; }
    }
}
