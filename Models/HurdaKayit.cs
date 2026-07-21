using System;
using System.ComponentModel.DataAnnotations;

namespace FireHurdaTakip.Models
{
    public class HurdaKayit
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tarih alanı zorunludur.")]
        [Display(Name = "Tarih")]
        public DateTime Tarih { get; set; }

        [Display(Name = "Toplam Plastik Hurdası (KG)")]
        public double ToplamPlastikHurdasiKg { get; set; }

        [Display(Name = "Alınan Sipariş No")]
        public string? AlinanSiparisNo { get; set; }

        [Display(Name = "Akümülatör Hurdası (KG)")]
        public double AkumulatorHurdasiKg { get; set; }

        [Display(Name = "İzabeye Gönderilen Hurda (KG)")]
        public double IzabeyeGonderilenHurdaKg { get; set; }

        [Display(Name = "P. Enjeksiyona Gönderilen Hurda (KG)")]
        public double PEnjeksiyonaGonderilenHurdaKg { get; set; }

        [Display(Name = "Ay")]
        public int Ay => Tarih.Month;
    }
}
