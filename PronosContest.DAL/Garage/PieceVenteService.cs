using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Garage
{
    public class PieceVenteService
    {
        #region Propriétés primitives
        [Key]
        public int ID { get; set; }

        [ForeignKey("PieceVente")]
        public int? PieceVenteID { get; set; }
        
        [StringLength(500)]
        [Column(TypeName = "VARCHAR")]
        public string Libelle { get; set; }
        
        public float PrixClientHT { get; set; }
        public float PrixClientTTC { get; set; }
        public int Quantite { get; set; }
        #endregion

        public PieceVenteService()
        {

        }

        #region Propriétés de navigation
        public virtual PieceVente PieceVente { get; set; }
        #endregion
    }
}
