using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect.Models
{
    [Table("themes")]
    public class Theme
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
