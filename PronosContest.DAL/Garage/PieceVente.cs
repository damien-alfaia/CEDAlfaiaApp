using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
namespace CEDAlfaiaApp.DAL.Garage
{
    public class PieceVente
    {
        #region Propriétés primitives
        [Key]
        public int ID { get; set; }

        [ForeignKey("Client")]
        public int? ClientID { get; set; }

        [ForeignKey("Voiture")]
        public int? VoitureID { get; set; }

        [ForeignKey("RendezVous")]
        public int? RendezVousID { get; set; }

        public int MainDOeuvreDuree { get; set; }
        public float MainDOeuvreMontantHoraire { get; set; }

        public DateTime? DateDevis { get; set; }
        public int? NumDevis { get; set; }

        public DateTime? DateFacture { get; set; }
        public int? NumFacture { get; set; }
        public int Kilometrage { get; set; }

        public bool IsFactureAnnule { get; set; }
        public DateTime? DateHeureAnnulation { get; set; }
        public string CommentaireAnnulation { get; set; }

        public bool IsValide { get; set; }
        public DateTime? DateHeureValidation { get; set; }

        public bool IsEnvoyeComptable { get; set; }

        public bool IsDevisEnvoye { get; set; }

        public float? Remise { get; set; }

        public float TotalTTC { get; set; }
        public float MontantTVA { get; set; }
        public float TotalHT { get; set; }
        public float BeneficeTTC { get; set; }
        public float ResteAPayer { get; set; }

        public float CalculerTotalTCC()
        {
            return CalculerTotalHT()/* * (1 + 0.2F)*/;
        }
        public float CalculerMontantTVA()
        {
            return this.CalculerTotalTCC() - this.CalculerTotalHT();
        }
        public float CalculerTotalHT()
        {
            float total = (MainDOeuvreDuree * (MainDOeuvreMontantHoraire / (/*0.2F +*/ 1)))
                + this.Lignes.Sum(l => l.PrixClientHT * l.Quantite)
                + this.Services.Sum(l => l.PrixClientHT * l.Quantite);
            if (this.Remise != null)
                total -= total * this.Remise.Value;
            return total;
        }
        public float CalculerBeneficeTTC()
        {
            return this.CalculerTotalTCC() - this.Lignes.Sum(l => l.PrixGarageTTC * l.Quantite);
        }
        public float CalculerResteAPayer()
        {
            return this.CalculerTotalTCC() - this.Paiements.Sum(p => p.Montant);
        }
        #endregion

        public PieceVente()
        {
            this.Lignes = new List<PieceVenteLigne>();
            this.Services = new List<PieceVenteService>();
            this.Paiements = new List<PieceVentePaiement>();
            this.Documents = new List<DocumentPieceVente>();
        }

        #region Propriétés de navigation
        public virtual Client Client { get; set; }
        public virtual Voiture Voiture { get; set; }
        public virtual RendezVous RendezVous { get; set; }
        public virtual ICollection<PieceVenteLigne> Lignes { get; set; }
        public virtual ICollection<PieceVenteService> Services { get; set; }
        public virtual ICollection<PieceVentePaiement> Paiements { get; set; }
        public virtual ICollection<DocumentPieceVente> Documents { get; set; }
        #endregion
    }
}
