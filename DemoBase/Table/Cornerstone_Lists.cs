using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoBase.Table
{
    [Table("Cornerstone_Lists")]
    public class CornerstoneLists
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }
}
