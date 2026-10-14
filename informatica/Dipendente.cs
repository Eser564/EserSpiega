using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progettinoProgettoso
{
    class Dipendente
    {
        private int matricola; //PK
        private string nome;
        private string cognome;
        private string codiceDipartimento; //FK

        public string CodiceDipartimento
        {
            get
            {
                return codiceDipartimento;
            }
            set
            {
                codiceDipartimento = value;
            }
        }

        public string Nome
        {
            get
            {
                return nome;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    nome = value;
                else
                    DBMS.StampaErrore("Errore >> Campo obbligatorio!");
            }
        }

        public string Cognome
        {
            get
            {
                return cognome;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    cognome = value;
                else
                    DBMS.StampaErrore("Errore >> Campo obbligatorio!");
            }
        }

        public int Matricola
        {
            get
            {
                return matricola;
            }
            set 
            {
                if (value > 0)
                    matricola = value;
                else
                    DBMS.StampaErrore("Errore >> Valore di matricola non valido!");
            }
        }

        public Dipendente(int m, string nome, string cognome, string codice)
        {
            Matricola = m;
            Nome = nome;
            Cognome = cognome;
            CodiceDipartimento = codice;
        }

    }
}
