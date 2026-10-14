using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progettinoProgettoso
{
    class Dipartimento
    {
        private string codiceDipartimento; //PK
        private string nomeDipartimento;
        private string città;

        public string CodiceDipartimento
        {
            get
            {
                return codiceDipartimento;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    codiceDipartimento = value;
                else
                    DBMS.StampaErrore("Errore >> Campo obbligatorio!");
            }
        }

        public string NomeDipartimento
        {
            get
            {
                return nomeDipartimento;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    nomeDipartimento = value;
                else
                    DBMS.StampaErrore("Errore >> Campo obbligatorio!");
            }
        }

        public string Città
        {
            get
            {
                return città;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    città = value;
                else
                    DBMS.StampaErrore("Errore >> Campo obbligatorio!");
            }
        }

        public Dipartimento(string codice, string nome, string città)
        {
            CodiceDipartimento = codice;
            NomeDipartimento = nome;
            Città = città;
        }
    }
}
