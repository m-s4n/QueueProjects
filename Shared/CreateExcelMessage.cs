using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    // buyuk verileri gondermiyoruz
    // worker service db'dn alacak
    public class CreateExcelMessage
    {
        public string UserId { get; set; }
        public int FileId { get; set; }

        //public List<T> Product { get; set; }
    }
}
