using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Garage
{
    public enum TypePaiement
    {
        [Display(Name = "Espèces")]
        Especes = 0,
        [Display(Name = "Chèque")]
        Cheque = 1,
        [Display(Name = "Carte bancaire")]
        CB = 2,
        [Display(Name = "Virement")]
        Virement = 3
    }
    public class PieceVentePaiement
    {
        #region Propriétés primitives
        [Key]
        public int ID { get; set; }

        [ForeignKey("PieceVente")]
        public int? PieceVenteID { get; set; }
        
        public TypePaiement TypePaiement { get; set; }
        
        public DateTime Date { get; set; }
        public float Montant { get; set; }
        #endregion

        public PieceVentePaiement()
        {

        }

        #region Propriétés de navigation
        public virtual PieceVente PieceVente { get; set; }
        #endregion
    }
}
