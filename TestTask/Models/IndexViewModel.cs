﻿namespace TestTask.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Catalog> Catalogs { get; set; }
        public Catalog CurrentCatalog { get; set; }
    }
}
