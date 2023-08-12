using System;
using System.Collections.Generic;

namespace FileCreatorWorkerService.Entities;

public partial class Urun
{
    public int Id { get; set; }

    public string Ad { get; set; } = null!;

    public string Numara { get; set; } = null!;

    public string? Renk { get; set; }

    public decimal? Ucret { get; set; }

    public int Tur { get; set; }

    public bool? IsAktif { get; set; }

    public DateTime OpTime { get; set; }

    public bool IsDeleted { get; set; }
}
