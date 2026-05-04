using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Parametrage
{
    public class Parametrage
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }
        
        public string NomEntreprise { get; set; }
        public string EmailEntreprise { get; set; }
        public string TelephoneEntreprise { get; set; }
        public string PortableEntreprise { get; set; }
        public Adresse Adresse { get; set; }
        public byte[] Logo { get; set; }
        public float TVA { get; set; }
        public float MainDOeuvreMontantHoraire { get; set; }
        public string EmailComptable { get; set; }
        public bool IsSavePieceDeVente { get; set; }

        public string SIRET { get; set; }
        public string CodeAPE { get; set; }
        public string TVAIntraCommunautaire { get; set; }
        public string LibelleBasDePage { get; set; }

        public string EmailEnvoiSMTP { get; set; }
        public string ProtocoleSMTP { get; set; }
        public int? PortSMTP { get; set; }
        public string ServeurSMTP { get; set; }
        public string EmailSMTP { get; set; }
        public string PasswordSMTP { get; set; }
        public bool IsSSL { get; set; }

        public float? ObjectifAnnuel { get; set; }

        public byte[] Entete { get; set; }
        #endregion

        public Parametrage()
		{

		}

        #region Propriétés de navigation
        #endregion
    }
}
