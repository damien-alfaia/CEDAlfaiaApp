using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Garage
{
    public class RendezVous
    {
        #region Propriétés primitives
        [Key]
        public int ID { get; set; }
                
        public string Sujet { get; set; }
        public DateTime? DateHeureDebut { get; set; }
        public DateTime? DateHeureFin { get; set; }
        public int? Duree { get; set; }
        public string Commentaire { get; set; }
        #endregion

        public RendezVous()
        {

        }

    }
}
