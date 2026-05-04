using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Garage
{
    public class PieceVenteLigne
    {
        #region Propriétés primitives
        [Key]
        public int ID { get; set; }

        [ForeignKey("PieceVente")]
        public int? PieceVenteID { get; set; }

        [ForeignKey("Fournisseur")]
        public int? FournisseurID { get; set; }

        [StringLength(500)]
        [Column(TypeName = "VARCHAR")]
        public string Libelle { get; set; }

        public float Remise { get; set; }
        public float PrixGarageHT { get; set; }
        public float PrixGarageTTC { get; set; }
        public float PrixClientHT { get; set; }
        public float PrixClientTTC { get; set; }
        public int Quantite { get; set; }
        #endregion

        public PieceVenteLigne()
        {

        }

        #region Propriétés de navigation
        public virtual PieceVente PieceVente { get; set; }
        public virtual Fournisseur Fournisseur { get; set; }
        #endregion
    }
}
