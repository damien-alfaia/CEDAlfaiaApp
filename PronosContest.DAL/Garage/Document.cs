using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Garage
{
    public enum TypeDocument
    {
        Devis = 0,
        Facture = 1,
        BonLivraisonFournisseur = 2,
        DevisFournisseur = 3
    }

    public class DocumentPieceVente
    {
        #region Propriétés primitives
        [Key]
        public int ID { get; set; }

        [ForeignKey("PieceVente")]
        public int? PieceVenteID { get; set; }

        public string Libelle { get; set; }

        public TypeDocument TypeDocument { get; set; }
        public byte[] Doc { get; set; }
        public string DocFormatFichierBase64 { get; set; }

        public DateTime? DateCreation { get; set; }
        public DateTime? DateModification { get; set; }
        #endregion

        public DocumentPieceVente()
        {

        }
        public DocumentPieceVente(int pPieceVenteID, string pLibelle, TypeDocument pTypeDocument, byte[] pDoc, string pDocFormatFichierBase64)
        {
            this.PieceVenteID = pPieceVenteID;
            this.Libelle = pLibelle;
            this.TypeDocument = pTypeDocument;
            this.Doc = pDoc;
            this.DocFormatFichierBase64 = pDocFormatFichierBase64;
        }

        #region Propriétés de navigation
        public virtual PieceVente PieceVente { get; set; }
        #endregion
    }
}
